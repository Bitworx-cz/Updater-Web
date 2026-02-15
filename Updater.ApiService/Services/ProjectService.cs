using Microsoft.EntityFrameworkCore;
using Updater.ApiService.Database;
using Updater.ApiService.Database.Models;

namespace Updater.ApiService.Services;

public class ProjectService(Context context, UserService userService)
{
    public async Task<Project> CreateProjectAsync(string name, string? description, string token)
    {
        var user = await userService.GetUserByTokenAsync(token)
            ?? throw new UnauthorizedAccessException("Invalid token");

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Projects.AddAsync(project);

        // Add user to project
        var userProject = new UserProject
        {
            UserId = user.Id,
            ProjectId = project.Id,
            CreatedAt = DateTime.UtcNow
        };

        await context.UserProjects.AddAsync(userProject);
        await context.SaveChangesAsync();

        return project;
    }

    public async Task<IEnumerable<Project>> GetUserProjectsAsync(string token)
    {
        var user = await userService.GetUserByTokenAsync(token)
            ?? throw new UnauthorizedAccessException("Invalid token");

        return await context.UserProjects
            .AsNoTracking()
            .Where(up => up.UserId == user.Id)
            .Select(up => up.Project)
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(Guid projectId, string token)
    {
        var user = await userService.GetUserByTokenAsync(token)
            ?? throw new UnauthorizedAccessException("Invalid token");

        var hasAccess = await context.UserProjects
            .AnyAsync(up => up.UserId == user.Id && up.ProjectId == projectId);

        if (!hasAccess)
            throw new UnauthorizedAccessException("Access denied to this project");

        return await context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted);
    }

    public async Task DeleteProjectAsync(Guid projectId, string token)
    {
        var user = await userService.GetUserByTokenAsync(token)
            ?? throw new UnauthorizedAccessException("Invalid token");

        var hasAccess = await context.UserProjects
            .AnyAsync(up => up.UserId == user.Id && up.ProjectId == projectId);

        if (!hasAccess)
            throw new UnauthorizedAccessException("Access denied to this project");

        var project = await context.Projects
            .Include(p => p.Groups)
                .ThenInclude(g => g.Devices)
            .Include(p => p.Groups)
                .ThenInclude(g => g.Softwares)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            throw new InvalidOperationException("Project not found");

        // Soft delete: mark project as deleted
        var deletedAt = DateTime.UtcNow;
        project.IsDeleted = true;
        project.DeletedAt = deletedAt;

        // Cascade soft delete: mark all groups and devices as deleted
        foreach (var group in project.Groups)
        {
            group.IsDeleted = true;
            group.DeletedAt = deletedAt;

            // Mark all devices in this group as deleted
            foreach (var device in group.Devices)
            {
                device.IsDeleted = true;
                device.DeletedAt = deletedAt;
            }
        }

        await context.SaveChangesAsync();
    }
}
