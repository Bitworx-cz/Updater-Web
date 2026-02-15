using Microsoft.AspNetCore.Mvc;
using Updater.ApiService.Services;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}/token")]
    public async Task<IActionResult> GetToken(string id)
    {
        var token = await _userService.GetTokenAsync(id);
        return Ok(token);
    }
}
