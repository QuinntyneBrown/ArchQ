using System.Security.Cryptography;
using System.Text.RegularExpressions;
using ArchQ.Application.Common;
using ArchQ.Application.Tenants.Commands.CreateTenant;
using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IGlobalUserRepository _globalUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVerificationTokenRepository _verificationTokenRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IAuditRepository _auditRepository;
    private readonly IMediator _mediator;

    private static readonly RegisterResponse SuccessResponse = new()
    {
        Message = "Registration successful. Please check your email to verify your account."
    };

    public RegisterCommandHandler(
        IGlobalUserRepository globalUserRepository,
        IUserRepository userRepository,
        IVerificationTokenRepository verificationTokenRepository,
        ITenantRepository tenantRepository,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IAuditRepository auditRepository,
        IMediator mediator)
    {
        _globalUserRepository = globalUserRepository;
        _userRepository = userRepository;
        _verificationTokenRepository = verificationTokenRepository;
        _tenantRepository = tenantRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _auditRepository = auditRepository;
        _mediator = mediator;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // TODO: Invite token support is deferred. When provided, inviteToken is currently ignored.
        // Future: validate invite token, associate user with existing tenant, and bypass org creation.

        var emailLower = request.Email.Trim().ToLowerInvariant();

        // Check if email already exists — return generic success to prevent enumeration
        if (await _globalUserRepository.EmailExistsAsync(emailLower))
        {
            return SuccessResponse;
        }

        var slug = GenerateSlug(request.OrganizationName);

        // Create tenant if it doesn't exist
        var existingTenant = await _tenantRepository.GetBySlugAsync(slug);
        string tenantId;

        if (existingTenant is null)
        {
            var tenantResponse = await _mediator.Send(new CreateTenantCommand
            {
                DisplayName = request.OrganizationName.Trim(),
                Slug = slug,
                CreatedBy = emailLower
            }, cancellationToken);

            tenantId = tenantResponse.Id;
        }
        else
        {
            tenantId = existingTenant.Id;
        }

        // Create user in tenant scope
        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = new User
        {
            Email = emailLower,
            FullName = request.FullName.Trim(),
            PasswordHash = passwordHash,
            Status = "pending_verification",
            Roles = new List<string> { "owner" },
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user, slug);

        // Create global user mapping
        var globalUser = new GlobalUser
        {
            Email = emailLower,
            Memberships = new List<GlobalUserMembership>
            {
                new()
                {
                    TenantId = tenantId,
                    TenantSlug = slug,
                    UserId = user.Id,
                    Status = "active"
                }
            },
            CreatedAt = DateTime.UtcNow
        };

        await _globalUserRepository.CreateAsync(globalUser);

        // Generate verification token
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var tokenHash = TokenHasher.ComputeSha256(rawToken);

        var verificationToken = new VerificationToken
        {
            TokenHash = tokenHash,
            UserId = user.Id,
            TenantSlug = slug,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            Used = false,
            CreatedAt = DateTime.UtcNow
        };

        // Store token hash on user for quick lookup
        user.EmailVerificationTokenHash = tokenHash;
        user.EmailVerificationExpiresAt = verificationToken.ExpiresAt;
        await _userRepository.UpdateAsync(user, slug);

        await _verificationTokenRepository.CreateAsync(verificationToken);

        // Send verification email with raw token
        await _emailService.SendVerificationEmailAsync(emailLower, user.FullName, rawToken);

        // Audit
        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = tenantId,
            Action = "UserRegistered",
            EntityType = "User",
            EntityId = user.Id,
            UserId = user.Id,
            Timestamp = DateTime.UtcNow,
            Details = $"User '{user.FullName}' registered with email '{emailLower}'."
        });

        return SuccessResponse;
    }

    private static string GenerateSlug(string orgName)
    {
        var slug = orgName.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"[^a-z0-9-]", "");
        slug = Regex.Replace(slug, @"-{2,}", "-");
        slug = slug.Trim('-');

        if (slug.Length < 3)
            slug = slug.PadRight(3, '0');
        if (slug.Length > 63)
            slug = slug[..63].TrimEnd('-');

        return slug;
    }

}
