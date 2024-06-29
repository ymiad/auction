using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Security;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

namespace Auction.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public AuthorizationBehaviour(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
    {
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attr = request.GetType().GetCustomAttribute<AuthorizeAttribute>();
        if (attr == null)
        {
            return await next();
        }

        HttpContext? httpContext = _httpContextAccessor.HttpContext ?? throw new UnauthorizedAccessException();
        var authHeader = httpContext.Request.Headers["Authorization"];
        if (string.IsNullOrWhiteSpace(authHeader))
        {
            throw new UnauthorizedAccessException();
        }

        try
        {
            var secret = "secret_secret_secret_secret_secret_secret_secret_secret";
            JwtSecurityToken? jwtToken = TokenValidator.ValidateToken(authHeader.ToString(), secret) ?? throw new UnauthorizedAccessException();
            var userId = new Guid(jwtToken.Claims.First(x => x.Type == "id").Value);

            using (var connection = _unitOfWork.Create())
            {
                var user = await connection.Repositories.UserRepository.GetById(userId);
                httpContext.Items["User"] = user;
            }

            return await next();
        }
        catch // TODO: catch exact exception
        {
            throw;
        }
    }
}