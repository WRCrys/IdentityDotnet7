namespace IdentityDotnet7.Api.Configuration;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}