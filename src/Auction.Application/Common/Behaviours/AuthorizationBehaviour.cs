using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Exceptions;
using Auction.Application.Common.Security;
using Auction.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Auction.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attr = request.GetType().GetCustomAttribute<AuthorizeAttribute>();
        if (attr is null)
        {
            return await next();
        }

        HttpContext? httpContext = _httpContextAccessor.HttpContext ?? throw new UnauthorizedAccessException();
        var authHeader = httpContext.Request.Headers[HttpContextConstants.AuthorizationHeader];
        if (string.IsNullOrWhiteSpace(authHeader))
        {
            throw new UnauthorizedAccessException();
        }

        try
        {
            var secret = "secret_secret_secret_secret_secret_secret_secret_secret";
            var tokenValidationResult = await JwtTokenHelper.ValidateToken(authHeader.ToString(), secret) ?? throw new UnauthorizedAccessException();
            if (tokenValidationResult.IsValid)
            {
                Guid userId = new(tokenValidationResult.Claims[JwtTokenConstants.UserId]?.ToString() ?? string.Empty);
                var roleStr = tokenValidationResult.Claims[JwtTokenConstants.Role]?.ToString();
                var parseRoleResult = int.TryParse(roleStr, out int role);

                if (!parseRoleResult || !attr.Role.HasFlag((Role)role))
                {
                    throw new ForbiddenAccessException();
                }
                using (var connection = _unitOfWork.Create())
                {
                    var user = await connection.Repositories.UserRepository.GetById(userId) ?? throw new UnauthorizedAccessException();
                    httpContext.Items[HttpContextConstants.UserId] = user.Id;
                    httpContext.Items[HttpContextConstants.Role] = (int)user.Role;
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