using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Updater.ApiService.Models;
using Updater.ApiService.Services;

namespace Updater.ApiService.Controllers;

[ApiController]
[Route("")]
public class SystemController(SoftwareService softwareService, DeviceService deviceService, UserService userService) : ControllerBase
{
    [HttpGet("devices/{token}")]
    public Task<IEnumerable<ChipApiModel>> GetDevices(string token)
        => deviceService.GetDevices(token);

    [HttpGet("ping")]
    public IActionResult Ping() => Ok("Pong");

    [HttpGet("clear/{token}")]
    public async Task<IActionResult> Clear(string token)
    {
        await softwareService.ClearOldSoftwares(token);
        return Ok();
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload()
    {
        try
        {
            var form = await Request.ReadFormAsync();
            var token = form["token"].ToString();
            var filename = form["filename"].ToString();
            var groupToken = form["groupToken"].ToString();
            var file = form.Files["file"];

            if (string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(filename) ||
                file == null)
            {
                return BadRequest("Missing required fields: token, filename, file");
            }

            if (string.IsNullOrWhiteSpace(groupToken))
                return BadRequest("Missing or invalid group token");

            var result = await softwareService.UploadSw(groupToken, token, filename, file);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/token")]
    public async Task<IActionResult> GetToken(string id)
    {
        var token = await userService.GetTokenAsync(id);
        return Ok(token);
    }

    //token, sketchName, ESP.getEfuseMac(), ESP.getChipModel()
    [HttpGet("{token}/{sketchName}/{deviceId}/{type}")]
    public async Task<IActionResult> OldGet(string token, string sketchName, string deviceId, string type)
    {
        var binary = await softwareService.GetSoftware(deviceId, token, null);
        if (binary.Length == 0)
            return NotFound();

        return File(binary, "application/octet-stream", "firmware.bin");
    }

    [HttpGet("ud/{token}/{sketchName}/{deviceId}/{type}")]
    public async Task<IActionResult> OldUpdateDone(string token, string sketchName, string deviceId, string type)
    {
         await softwareService.SetUpdateDone(deviceId, token);
        return Ok();
    }
}