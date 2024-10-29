
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.Helpers;
using BackEnd.Models.FrontEndModels;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Services;

    public class AuthService
{
    public static string GenerateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(AuthSettings.PrivateKey);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = credentials,
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaims(User user)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.Name, user.email));

        return claims;
    }
}


