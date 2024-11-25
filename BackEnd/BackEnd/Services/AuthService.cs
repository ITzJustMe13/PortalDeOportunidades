﻿
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackEnd.Interfaces;
using BackEnd.Models.BackEndModels;
using BackEnd.Models.FrontEndModels;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Services;

public class AuthService : IAuthService
{
    /// <summary>
    /// Function that Generates a Token using Jwt and adds an expiration date to it
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Returns the token in a String form</returns>
    public string GenerateToken(UserModel user)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new(ClaimTypes.Email, user.Email),
    };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = credentials,
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}


