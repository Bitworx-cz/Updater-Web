using Microsoft.AspNetCore.Mvc;
using Updater.ApiService.Services;
using Updater.ApiService.Database;
using Microsoft.AspNetCore.Http.HttpResults;

[ApiController]
[Route("device")]
public class DeviceController(SoftwareService softwareService) : ControllerBase
{

    [HttpGet("check-update/{token}/{deviceId}")]
    public async Task<IActionResult> CheckUpdate(string token, string deviceId)
    {
        try
        {
            var available = await softwareService.IsUpdateAvailable(deviceId, token);
            return available ? Ok() : NotFound();
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet("challenge/{token}/{deviceId}")]
    public async Task<IActionResult> Challenge(string token, string deviceId)
    {
        try
        {
            var challenge = await softwareService.Challenge(deviceId, token);

            return Ok(new { challenge });
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet("download/{token}/{deviceId}")]
    public async Task<IActionResult> Download(string token, string deviceId, [FromQuery] string? hmac)
    {
        try
        {
            var binary = await softwareService.GetSoftware(deviceId, token, hmac);
            if (binary.Length == 0)
                return NotFound();

            return File(binary, "application/octet-stream", "firmware.bin");
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost("update-done/{token}/{deviceId}")]
    public async Task<IActionResult> UpdateDone(string token, string deviceId)
    {
        try
        {
            await softwareService.SetUpdateDone(deviceId, token);
            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }
}
