using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auction.Application.Users.Commands.Login;

public class LoginCommand : IRequest<string>
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<string> Handle(LoginCommand command, CancellationToken cancellationToken)
    {

        using var connection = _unitOfWork.Create();

        var user = await connection.Repositories.UserRepository.GetByCredentials(command.Username, command.Password);

        if (user is null)
        {
            return string.Empty;
        }

        var token = GenerateJwtToken(user.Id, user.Role);

        return token;
    }

    public string GenerateJwtToken(Guid userId, Role role)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var key = Encoding.UTF8.GetBytes("secret_secret_secret_secret_secret_secret_secret_secret");


        List<Claim> claims =
        [
            new Claim("user_id", userId.ToString()),
            new Claim("user_role", ((int)role).ToString()),
        ];

        //ClaimsIdentity cl = new ClaimsIdentity()

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            //Expires = new TimeSpan(DateTime.Now.AddDays(90).Ticks - DateTime.Now.Ticks).TotalSeconds,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return token;
    }
}
