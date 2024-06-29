using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Auction.Application.Common.Security;

public static class TokenValidator
{
    public static JwtSecurityToken? ValidateToken(string authHeader, string secret)
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

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;

        return jwtToken;
    }
}
