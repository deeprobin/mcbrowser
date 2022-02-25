using Microsoft.AspNetCore.Builder;

namespace MinecraftServerlist.InternalApi.Extensions;

public static class WebApplicationExtensions
{
    public static void MapInternalApi(this WebApplication app)
    {
        app.MapControllers();
    }
}