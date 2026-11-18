# Multi-stage Dockerfile for ASP.NET Core REST API (.NET 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY admin/backend_rest_api_csharp/Jatetxea.Api.csproj admin/backend_rest_api_csharp/
RUN dotnet restore admin/backend_rest_api_csharp/Jatetxea.Api.csproj

# Copy full source and publish release
COPY admin/backend_rest_api_csharp/ admin/backend_rest_api_csharp/
WORKDIR /src/admin/backend_rest_api_csharp
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Non-root user for security (Zero-Trust) - .NET built-in app user
USER $APP_UID

COPY --from=build /app/publish .

# Environment configuration
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000

ENTRYPOINT ["dotnet", "Jatetxea.Api.dll"]
