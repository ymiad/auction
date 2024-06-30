using Auction.Application.Common.Abstractions.UnitOfWork;
using Auction.Application.Common.Exceptions;
using Auction.Application.Common.Security;
using Auction.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Auction.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse>(
    IHttpContextAccessor httpContextAccessor,
    IUnitOfWork unitOfWork,
    ILogger<AuthorizationBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<AuthorizationBehaviour<TRequest, TResponse>> _logger = logger;

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
            _logger.LogWarning("{Message}", "Authorization header should be filled");
            throw new UnauthorizedAccessException();
        }

        var secret = "secret_secret_secret_secret_secret_secret_secret_secret";
        var tokenValidationResult = await JwtTokenHelper.ValidateToken(authHeader.ToString(), secret);

        if (tokenValidationResult is null || !tokenValidationResult.IsValid)
        {
            _logger.LogWarning("{Message}", "Token is not valid");
            throw new ForbiddenAccessException();
        }

        Guid userId = new(tokenValidationResult.Claims[JwtTokenConstants.UserId]?.ToString() ?? string.Empty);
        var roleStr = tokenValidationResult.Claims[JwtTokenConstants.Role]?.ToString();
        var parseRoleResult = int.TryParse(roleStr, out int role);

        if (!parseRoleResult || !attr.Role.HasFlag((Role)role))
        {
            _logger.LogWarning("{Message}", "Role is incorrect");
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
}