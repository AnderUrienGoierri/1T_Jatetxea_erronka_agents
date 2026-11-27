using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Jatetxea.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace Jatetxea.Api.Helpers;

/// <summary>
/// JWT Bearer tokenak (HS256) sortu eta balidatzeko laguntza-klasea.
/// Rol bidezko baimenak (RBAC) ahalbidetzen ditu: Admin, AdminIT, Kudeatzailea, Zerbitzaria, Sukaldaria...
/// </summary>
public static class JwtHelper
{
    private const string DefaultSecret = "1T_Jatetxea_GoierriEskola_JWT_Segurtasun_Gako_2026_SuperSecret!";
    private const string DefaultIssuer = "1T_Jatetxea_Api";
    private const string DefaultAudience = "1T_Jatetxea_Bezeroak";

    public static string LortuGakoa(IConfiguration config)
    {
        string? envKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        if (!string.IsNullOrWhiteSpace(envKey) && envKey.Length >= 32)
            return envKey;

        string? cfgKey = config["Jwt:SecretKey"];
        return !string.IsNullOrWhiteSpace(cfgKey) && cfgKey.Length >= 32
            ? cfgKey
            : DefaultSecret;
    }

    public static string LortuIssuer(IConfiguration config)
        => config["Jwt:Issuer"] ?? DefaultIssuer;

    public static string LortuAudience(IConfiguration config)
        => config["Jwt:Audience"] ?? DefaultAudience;

    public static TokenValidationParameters LortuBalidazioParametroak(IConfiguration config)
    {
        var keyBytes = Encoding.UTF8.GetBytes(LortuGakoa(config));
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = LortuIssuer(config),
            ValidateAudience = true,
            ValidAudience = LortuAudience(config),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name
        };
    }

    /// <summary>
    /// Langile autentifikatuarentzat HS256 JWT tokena sortzen du.
    /// </summary>
    public static (string Token, DateTime IraungitzeData) SortuTokena(Langilea langilea, IConfiguration config)
    {
        int orduak = int.TryParse(config["Jwt:IraungitzeOrduak"], out int h) && h > 0 ? h : 8;
        DateTime iraungitzea = DateTime.UtcNow.AddHours(orduak);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, langilea.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, langilea.Erabiltzailea),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, langilea.Id.ToString()),
            new Claim(ClaimTypes.Name, langilea.Izena),
            new Claim(ClaimTypes.Role, langilea.Rola),
            new Claim("erabiltzailea", langilea.Erabiltzailea),
            new Claim("saila", langilea.Saila)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(LortuGakoa(config)));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: LortuIssuer(config),
            audience: LortuAudience(config),
            claims: claims,
            expires: iraungitzea,
            signingCredentials: credentials);

        string token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return (token, iraungitzea);
    }
}
