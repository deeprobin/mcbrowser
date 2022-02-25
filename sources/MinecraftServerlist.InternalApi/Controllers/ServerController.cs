using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinecraftServerlist.InternalApi.Common.ResponseObjects;
using MinecraftServerlist.Services.Abstractions;

namespace MinecraftServerlist.InternalApi.Controllers;

[ApiController]
[Route("api/internal/[controller]/[action]")]
public sealed class ServerController : Controller
{
    private readonly IServerService _serverService;

    public ServerController(IServerService serverService)
    {
        _serverService = serverService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetTopServers([FromQuery] int amount = 30, [FromQuery] int skip = 0, CancellationToken cancellationToken = default)
    {
        var response = new List<ServerResponse>();
        await foreach (var server in _serverService.GetTopServersAsync(amount, skip, cancellationToken))
        {
            var responseModel = await Mapper.MapToServerResponseAsync(server);
            response.Add(responseModel);
        }

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("/api/internal/Server/CountServers")]
    public async Task<IActionResult> CountServers(CancellationToken cancellationToken = default)
    {
        var result = await _serverService.CountServersAsync(cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetNewestServers([FromQuery] int amount = 30, [FromQuery] int skip = 0, CancellationToken cancellationToken = default)
    {
        var response = new List<ServerResponse>();
        await foreach (var server in _serverService.GetNewestServersAsync(amount, skip, cancellationToken))
        {
            var responseModel = await Mapper.MapToServerResponseAsync(server);
            response.Add(responseModel);
        }

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetServer([FromQuery] int id, CancellationToken cancellationToken)
    {
        var server = await _serverService.GetServerByIdAsync(id, cancellationToken);

        if (server is null)
        {
            return NoContent();
        }

        var responseModel = await Mapper.MapToAdvancedServerResponseAsync(server);
        return Ok(responseModel);
    }
}