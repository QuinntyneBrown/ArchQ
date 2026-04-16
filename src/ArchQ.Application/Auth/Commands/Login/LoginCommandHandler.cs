using ArchQ.Core.Entities;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using ArchQ.Application.Common;
using MediatR;

namespace ArchQ.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IGlobalUserRepository _globalUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IEmailService _emailService;

    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan FailedAttemptWindow = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public LoginCommandHandler(
        IGlobalUserRepository globalUserRepository,
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IAuditRepository auditRepository,
        IEmailService emailService)
    {
        _globalUserRepository = globalUserRepository;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _auditRepository = auditRepository;
        _emailService = emailService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var emailLower = request.Email.Trim().ToLowerInvariant();

        // Look up global user
        var globalUser = await _globalUserRepository.GetByEmailAsync(emailLower);
        if (globalUser is null)
        {
            throw new DomainException("INVALID_CREDENTIALS", "The email or password you entered is incorrect.");
        }

        // Get first membership
        var membership = globalUser.Memberships.FirstOrDefault();
        if (membership is null)
        {
            throw new DomainException("INVALID_CREDENTIALS", "The email or password you entered is incorrect.");
        }

        // Get tenant
        var tenant = await _tenantRepository.GetByIdAsync(membership.TenantId);
        if (tenant is null)
        {
            throw new DomainException("INVALID_CREDENTIALS", "The email or password you entered is incorrect.");
        }

        // Get user from tenant scope
        var user = await _userRepository.GetByIdAsync(membership.UserId, membership.TenantSlug);
        if (user is null)
        {
            throw new DomainException("INVALID_CREDENTIALS", "The email or password you entered is incorrect.");
        }

        // Check user status
        if (user.Status == "pending_verification")
        {
            throw new DomainException("PENDING_VERIFICATION", "Please verify your email first.");
        }

        if (user.Status != "active")
        {
            throw new DomainException("ACCOUNT_INACTIVE", "Your account is not active.");
        }

        // Check lockout
        if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
        {
            throw new DomainException("ACCOUNT_LOCKED", "Account temporarily locked. Try again later.");
        }

        // Verify password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            // Handle failed attempt
            // Reset window if first attempt was outside the window
            if (user.FirstFailedAttemptAt.HasValue &&
                DateTime.UtcNow - user.FirstFailedAttemptAt.Value > FailedAttemptWindow)
            {
                user.FailedLoginAttempts = 0;
                user.FirstFailedAttemptAt = null;
            }

            user.FailedLoginAttempts++;

            if (user.FirstFailedAttemptAt is null)
            {
                user.FirstFailedAttemptAt = DateTime.UtcNow;
            }

            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockedUntil = DateTime.UtcNow.Add(LockoutDuration);
                user.FailedLoginAttempts = 0;
                user.FirstFailedAttemptAt = null;

                await _emailService.SendAccountLockedEmailAsync(user.Email, user.FullName);
            }

            await _userRepository.UpdateAsync(user, membership.TenantSlug);

            throw new DomainException("INVALID_CREDENTIALS", "The email or password you entered is incorrect.");
        }

        // Successful login — reset counters
        user.FailedLoginAttempts = 0;
        user.FirstFailedAttemptAt = null;
        user.LockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, membership.TenantSlug);

        // Create tokens
        var accessToken = _tokenService.CreateAccessToken(user, tenant.Id, tenant.Slug, user.Roles);
        var family = Guid.NewGuid().ToString();
        var refreshTokenJwt = _tokenService.CreateRefreshToken(user.Id, family);

        // Store refresh token
        var refreshTokenPayload = _tokenService.VerifyRefreshToken(refreshTokenJwt);
        var tokenHash = TokenHasher.ComputeSha256(refreshTokenPayload!.TokenId);

        var refreshToken = new Core.Entities.RefreshToken
        {
            TokenHash = tokenHash,
            Family = family,
            UserId = user.Id,
            TenantSlug = membership.TenantSlug,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false
        };

        await _refreshTokenRepository.CreateAsync(refreshToken);

        // Audit
        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = tenant.Id,
            Action = "LOGIN_SUCCESS",
            EntityType = "User",
            EntityId = user.Id,
            UserId = user.Id,
            Timestamp = DateTime.UtcNow,
            Details = $"User '{user.FullName}' logged in.",
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent
        });

        // Build memberships list
        var memberships = globalUser.Memberships.Select(m => new LoginMembershipDto
        {
            TenantId = m.TenantId,
            TenantSlug = m.TenantSlug,
            UserId = m.UserId,
            Status = m.Status
        }).ToList();

        return new LoginResponse
        {
            User = new LoginUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = user.Roles
            },
            Tenant = new LoginTenantDto
            {
                Id = tenant.Id,
                Slug = tenant.Slug,
                DisplayName = tenant.DisplayName
            },
            Memberships = memberships,
            AccessToken = accessToken,
            RefreshToken = refreshTokenJwt
        };
    }
}
