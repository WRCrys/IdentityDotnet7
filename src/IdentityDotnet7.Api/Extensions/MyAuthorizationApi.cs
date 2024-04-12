using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityDotnet7.Api.Extensions;

public class MyAuthorizationApi
{
    public static bool CheckUser(HttpContext context, string profile, string action)
    {
        var result = context.User.Identity.IsAuthenticated && context.User.Claims.Any(c => c.Type.Contains(profile) && c.Value.Contains(action));

        return result;
    }
}

public class ProfileAuthorizeAttribute : TypeFilterAttribute
{
    public ProfileAuthorizeAttribute(string profile, string action) : base(typeof(RequirementClaimFilter))
    {
        Arguments = new object[] { new Claim(profile, action) };
    }
}

public class RequirementClaimFilter : IAuthorizationFilter
{
    private readonly Claim _claim;

    public RequirementClaimFilter(Claim claim)
    {
        _claim = claim;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
            context.Result = new StatusCodeResult(401);

        if (!MyAuthorizationApi.CheckUser(context.HttpContext, _claim.Type, _claim.Value))
            context.Result = new StatusCodeResult(403);
    }
}