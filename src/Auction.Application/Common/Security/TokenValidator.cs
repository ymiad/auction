using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Auction.Application.Common.Security;

public static class TokenValidator
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
}
