using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.RegularExpressions;
using Updater.ApiService.Database;
using Updater.ApiService.Database.Models;
using Group = Updater.ApiService.Database.Models.Group;

namespace Updater.ApiService.Services;

public class GroupService(Context context, UserService userService, ProjectService projectService)
{
    public async Task<Group> CreateGroupAsync(Guid projectId, string name, string? description, string userToken)
    {
        // Verify user has access to project
        await projectService.GetProjectByIdAsync(projectId, userToken);

        var group = new Group
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Token = Helpers.GenerateToken()
        };

        await context.Groups.AddAsync(group);
        await context.SaveChangesAsync();

        return group;
    }

    public async Task<IEnumerable<Group>> GetProjectGroupsAsync(Guid projectId, string userToken)
    {
        // Verify user has access to project
        await projectService.GetProjectByIdAsync(projectId, userToken);

        return await context.Groups
            .AsNoTracking()
            .Include(g => g.TargetSoftware)
            .Include(g => g.Devices)
            .Where(g => g.ProjectId == projectId && !g.IsDeleted)
            .ToListAsync();
    }

    public async Task<Group?> GetGroupByIdAsync(Guid groupId)
    {
        var group = await context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == groupId && !g.IsDeleted);

        if(group is not null && string.IsNullOrEmpty(group.Token))
        {
            group.Token = Helpers.GenerateToken();
            context.Update(group);
            await context.SaveChangesAsync();
        }

        return group;
    }

    public async Task<Group?> GetGroupDetailAsync(Guid groupId, string userToken)
    {
        var group = await context.Groups
            .AsNoTracking()
            .Include(g => g.TargetSoftware)
            .Include(g => g.Devices)
                .ThenInclude(d => d.CurrentSoftware)
            .FirstOrDefaultAsync(g => g.Id == groupId && !g.IsDeleted);

        if (group == null)
        {
            return null;
        }

        // Verify user has access to project
        await projectService.GetProjectByIdAsync(group.ProjectId, userToken);

        if (string.IsNullOrEmpty(group.Token))
        {
            group.Token = Helpers.GenerateToken();
            context.Update(group);
            await context.SaveChangesAsync();
        }

        return group;
    }

    public async Task<Database.Models.Group?> GetGroupByTokenAsync(string groupToken)
    {
        return await context.Groups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Token == groupToken);
    }

    public async Task SetTargetSoftwareAsync(Guid groupId, Guid softwareId, string userToken)
    {
        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == groupId && !g.IsDeleted)
            ?? throw new InvalidOperationException("Group not found");

        // Verify user has access to project
        await projectService.GetProjectByIdAsync(group.ProjectId, userToken);

        group.TargetSoftwareId = softwareId;
        group.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task DeleteGroupAsync(Guid groupId, string userToken)
    {
        var group = await context.Groups
            .Include(g => g.Devices)
            .Include(g => g.Softwares)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
            throw new InvalidOperationException("Group not found");

        // Verify user has access to project
        await projectService.GetProjectByIdAsync(group.ProjectId, userToken);

        // Soft delete: mark group as deleted
        group.IsDeleted = true;
        group.DeletedAt = DateTime.UtcNow;

        // Cascade soft delete: mark all devices in this group as deleted
        var deletedAt = DateTime.UtcNow;
        foreach (var device in group.Devices)
        {
            device.IsDeleted = true;
            device.DeletedAt = deletedAt;
        }

        await context.SaveChangesAsync();
    }

    internal async Task SetSecretAsync(Guid id, string secret, bool useHMAC)
    {
        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == id) ?? throw new InvalidOperationException("Group not found");

        group.HMACKey = secret;
        group.UseHMAC = useHMAC;

        await context.SaveChangesAsync();
    }
}
