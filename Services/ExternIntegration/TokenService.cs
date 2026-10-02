using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace app_pm_back.Services;

public class TokenService 
{

    public static string GenerateToken()
    {
        string _secret;
        _secret = Environment.GetEnvironmentVariable("TWINS_KEY") ?? 
                  throw new InvalidOperationException("TWINS_KEY não foi configurada");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: "http://0.0.0.0:5000",
            audience: "http://0.0.0.0:5000",
            expires: DateTime.UtcNow.AddMinutes(5),
            claims: new[]
            {
                new Claim("service", "UnitAnalyticsBack")
            },
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}