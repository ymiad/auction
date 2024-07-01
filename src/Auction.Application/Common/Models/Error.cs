namespace Auction.Application.Common.Models;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

public static class AuthError
{
    public static readonly Error Unauthorized = new($"{nameof(AuthError)}.{nameof(Unauthorized)}", "Unauthorized");
    public static readonly Error InvalidCredentials = new($"{nameof(AuthError)}.{nameof(InvalidCredentials)}", "Invalid credentials");
}

public static class UserError
{
    public static readonly Error NotFound = new($"{nameof(UserError)}.{nameof(NotFound)}", "User not found");
    public static readonly Error AlreadyExists = new($"{nameof(UserError)}.{nameof(AlreadyExists)}", "User already exists");
    public static readonly Error Banned = new($"{nameof(UserError)}.{nameof(Banned)}", "User is banned");
}

public static class AccountError
{
    public static readonly Error NotFound = new($"{nameof(AccountError)}.{nameof(NotFound)}", "Accout not found");
}

public static class UserBetError
{
    public static readonly Error AmmountLessThanPrice = new($"{nameof(UserBetError)}.{nameof(AmmountLessThanPrice)}", "User bet amount less than lot start price or current bet ammount");
    public static readonly Error SameUser = new($"{nameof(UserBetError)}.{nameof(SameUser)}", "Can not place bet for own lot");
}

public static class LotError
{
    public static readonly Error NotFound = new($"{nameof(LotError)}.{nameof(NotFound)}", "Lot not found");

    public static readonly Error Archived = new($"{nameof(LotError)}.{nameof(Archived)}", "Lot is archived");

    public static readonly Error Sold = new($"{nameof(LotError)}.{nameof(Sold)}", "Lot is sold");

    public static readonly Error TradingNotStarted = new($"{nameof(LotError)}.{nameof(TradingNotStarted)}", "Trading is not started");
}