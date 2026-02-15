using Microsoft.AspNetCore.Mvc;
using Updater.ApiService.Services;

namespace Updater.ApiService.Controllers;

[ApiController]
[Route("projects")]
public class ProjectController(ProjectService projectService, GroupService groupService) : ControllerBase
{

    [HttpGet("{token}")]
    public async Task<IActionResult> GetUserProjects(string token)
    {
        try
        {
            var list = await projectService.GetUserProjectsAsync(token);
            return Ok(list);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("{token}")]
    public async Task<IActionResult> CreateProject(string token)
    {
        try
        {
            var form = await Request.ReadFormAsync();
            var name = form["name"].ToString();
            var description = form["description"].ToString();

            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Project name is required");

            var project = await projectService.CreateProjectAsync(
                name,
                string.IsNullOrWhiteSpace(description) ? null : description,
                token
            );

            return Ok(new
            {
                project.Id,
                project.Name,
                project.Description,
                project.CreatedAt,
                project.UpdatedAt
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpDelete("{projectId:guid}/{token}")]
    public async Task<IActionResult> DeleteProject(Guid projectId, string token)
    {
        try
        {
            await projectService.DeleteProjectAsync(projectId, token);
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

    [HttpGet("{projectId:guid}/groups/{token}")]
    public async Task<IActionResult> GetProjectGroups(Guid projectId, string token)
    {
        try
        {
            var groups = await groupService.GetProjectGroupsAsync(projectId, token);

            var result = groups.Select(g => new
            {
                g.Id,
                g.ProjectId,
                g.Name,
                g.Description,
                g.TargetSoftwareId,
                g.CreatedAt,
                g.UpdatedAt,
                Devices = g.Devices.Select(d => new { d.Id }).ToList(),
                TargetSoftware = g.TargetSoftware == null ? null : new
                {
                    g.TargetSoftware.Id,
                    g.TargetSoftware.Name,
                    g.TargetSoftware.VerMajor
                }
            });

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("{projectId:guid}/groups/{token}")]
    public async Task<IActionResult> CreateGroup(Guid projectId, string token)
    {
        try
        {
            var form = await Request.ReadFormAsync();
            var name = form["name"].ToString();
            var description = form["description"].ToString();

            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Group name is required");

            var group = await groupService.CreateGroupAsync(
                projectId,
                name,
                string.IsNullOrWhiteSpace(description) ? null : description,
                token
            );

            return Ok(new
            {
                group.Id,
                group.ProjectId,
                group.Name,
                group.Description,
                group.CreatedAt,
                group.UpdatedAt
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}