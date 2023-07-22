namespace IdentityDotnet7.Api.Configuration;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Secret { get; set; } = null!;

    public int ExpireMinutes { get; set; }

    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;
}