using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Exceptions;
using Auction.Application.Common.Security;
using Auction.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Data;
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
            var tokenValidationResult = await TokenValidator.ValidateToken(authHeader.ToString(), secret) ?? throw new UnauthorizedAccessException();
            if (tokenValidationResult.IsValid)
            {
                Guid userId = new Guid(tokenValidationResult.Claims["user_id"]?.ToString() ?? string.Empty);
                var roleStr = tokenValidationResult.Claims["user_role"]?.ToString();
                var parseRoleResult = int.TryParse(roleStr, out int role);

                if (!parseRoleResult || !attr.Role.HasFlag((Role)role))
                {
                    throw new ForbiddenAccessException();
                }
                using (var connection = _unitOfWork.Create())
                {
                    var user = await connection.Repositories.UserRepository.GetById(userId);
                    httpContext.Items["user_id"] = user.Id;
                    httpContext.Items["user_role"] = (int)user.Role;
                }

                return await next();
            }

            throw new ForbiddenAccessException();
        }
        catch // TODO: catch exact exception
        {
            throw;
        }
    }
}