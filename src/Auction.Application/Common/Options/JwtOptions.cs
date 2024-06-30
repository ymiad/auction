namespace Auction.Application.Common.Options;

public class JwtOptions
{
    public const string Section = nameof(JwtOptions);
    public string Secret { get; set; } = string.Empty;
}
