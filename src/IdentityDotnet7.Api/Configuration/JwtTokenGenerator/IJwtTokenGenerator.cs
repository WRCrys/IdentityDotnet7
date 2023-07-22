using Microsoft.AspNetCore.Identity;

namespace IdentityDotnet7.Api.Configuration.JwtTokenGenerator;

public interface IJwtTokenGenerator
{
    string GenerateToken(IdentityUser user);
}