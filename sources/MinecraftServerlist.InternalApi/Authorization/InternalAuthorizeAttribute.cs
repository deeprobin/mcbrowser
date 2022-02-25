using Microsoft.AspNetCore.Authorization;

namespace MinecraftServerlist.InternalApi.Authorization;

internal sealed class InternalAuthorizeAttribute : AuthorizeAttribute
{
    public InternalAuthorizeAttribute()
    {
        Policy = "InternalAuthorization";
    }
}