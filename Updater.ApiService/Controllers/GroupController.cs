using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Updater.ApiService.Database;
using Updater.ApiService.Services;

namespace Updater.ApiService.Controllers;

[ApiController]
[Route("groups")]
public class GroupController(GroupService groupService, SoftwareService softwareService, Context context) : ControllerBase
{
    [HttpDelete("{groupId:guid}/{token}")]
    public async Task<IActionResult> Delete(Guid groupId, string token)
    {
        try
        {
            await groupService.DeleteGroupAsync(groupId, token);
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{groupId:guid}/{token}")]
    public async Task<IActionResult> Detail(Guid groupId, string token)
    {
        try
        {
            var group = await groupService.GetGroupDetailAsync(groupId, token);
            if (group == null)
                return NotFound();

            var deviceIds = group.Devices.Select(d => d.Id).ToList();

            var lastActivities = await context.DeviceActivities
                .Where(da => deviceIds.Contains(da.DeviceId))
                .GroupBy(da => da.DeviceId)
                .Select(g => new
                {
                    DeviceId = g.Key,
                    LastActivity = g.Max(da => da.Timestamp)
                })
                .ToDictionaryAsync(x => x.DeviceId, x => x.LastActivity);

            var dto = new
            {
                group.Id,
                group.ProjectId,
                group.Name,
                group.Description,
                group.TargetSoftwareId,
                group.CreatedAt,
                group.UpdatedAt,
                group.Token,
                group.UseHMAC,
                TargetSoftware = group.TargetSoftware == null ? null : new
                {
                    group.TargetSoftware.Id,
                    group.TargetSoftware.Name,
                    group.TargetSoftware.VerMajor
                },
                Devices = group.Devices.Select(d => new
                {
                    d.Id,
                    d.DeviceId,
                    d.MacAddress,
                    CurrentSoftware = d.CurrentSoftware == null ? null : new
                    {
                        d.CurrentSoftware.Id,
                        d.CurrentSoftware.Name,
                        d.CurrentSoftware.VerMajor
                    },
                    LastActivity = lastActivities.TryGetValue(d.Id, out DateTime value) ? value : DateTime.MinValue,
                })
            };

            return Ok(dto);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpGet("{groupId:guid}/software/{token}")]
    public async Task<IActionResult> GetSoftware(Guid groupId, string token)
    {
        try
        {
            var softwares = await softwareService.GetGroupSoftwareVersions(groupId, token);

            return Ok(softwares.Select(s => new
            {
                s.Id,
                s.Name,
                s.VerMajor,
                s.CreatedAt
            }));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPut("{groupId:guid}/target-software/{token}")]
    public async Task<IActionResult> SetTargetSoftware(Guid groupId, string token)
    {
        try
        {
            var form = await Request.ReadFormAsync();
            if (!Guid.TryParse(form["softwareId"], out var softwareId))
                return BadRequest("Invalid software ID");

            await groupService.SetTargetSoftwareAsync(groupId, softwareId, token);
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("{id}/set-secret")]
    public async Task<IActionResult> SetSecret(Guid id, [FromBody] SetSecretDto dto)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid group ID.");

        await groupService.SetSecretAsync(id, dto.Secret, dto.UseHMAC);

        return Ok(new { success = true });
    }

    public class SetSecretDto
    {
        public Guid GroupId { get; set; }
        public string Secret { get; set; } = string.Empty;
        public bool UseHMAC { get; set; }
    }
}