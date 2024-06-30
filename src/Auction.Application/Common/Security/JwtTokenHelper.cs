using Auction.Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Auction.Application.Common.Security;

public static class JwtTokenHelper
{
    public async static Task<TokenValidationResult?> ValidateToken(string authHeader, string secret)
    {
        if (string.IsNullOrEmpty(authHeader))
        {
            return null;
        }

        var splitted = authHeader.Split(" ");
        if (splitted.Length != 2)
        {
            return null;
        }

        var token = splitted[1];

        var tokenHandler = new JsonWebTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);
        var tokenValidationResult = await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        });

        return tokenValidationResult;
    }

    public static string GenerateJwtToken(Guid userId, Role role, string secret)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);


        List<Claim> claims =
        [
            new Claim(JwtTokenConstants.UserId, userId.ToString()),
            new Claim(JwtTokenConstants.Role, ((int)role).ToString()),
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return token;
    }
}
