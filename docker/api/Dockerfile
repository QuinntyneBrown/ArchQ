FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/ArchQ.Api/ArchQ.Api.csproj", "src/ArchQ.Api/"]
COPY ["src/ArchQ.Application/ArchQ.Application.csproj", "src/ArchQ.Application/"]
COPY ["src/ArchQ.Core/ArchQ.Core.csproj", "src/ArchQ.Core/"]
COPY ["src/ArchQ.Infrastructure/ArchQ.Infrastructure.csproj", "src/ArchQ.Infrastructure/"]
RUN dotnet restore "src/ArchQ.Api/ArchQ.Api.csproj"
COPY . .
RUN dotnet publish "src/ArchQ.Api/ArchQ.Api.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ArchQ.Api.dll"]
