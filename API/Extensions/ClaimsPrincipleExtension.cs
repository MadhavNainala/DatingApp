using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipleExtension
{
    public static string GetUserId(this ClaimsPrincipal user ){
        return user.FindFirst(ClaimTypes.NameIdentifier).Value;
    }

    public static string GetUsername(this ClaimsPrincipal user ){
        return user.FindFirst(ClaimTypes.Name).Value;
    }
}
