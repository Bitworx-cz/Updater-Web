using Microsoft.EntityFrameworkCore;
using Updater.ApiService.Database;
using Updater.ApiService.Database.Models;
using Updater.ApiService.Models;

namespace Updater.ApiService.Services;

public class DeviceService(Context context, UserService userService, ProjectService projectService)
{
    public async Task<Device> RegisterDeviceAsync(Guid groupId, string deviceId, string macAddress, string token)
    {
        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == groupId)
            ?? throw new InvalidOperationException("Group not found");

        // Verify user has access to project
        await projectService.GetProjectByIdAsync(group.ProjectId, token);

        // Check if device already exists
        var existingDevice = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        if (existingDevice != null)
            throw new InvalidOperationException("Device already registered");

        var device = new Device
        {
            Id = Guid.NewGuid(),
            DeviceId = deviceId,
            MacAddress = macAddress,
            GroupId = groupId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Devices.AddAsync(device);
        await context.SaveChangesAsync();

        return device;
    }

    public async Task<IEnumerable<ChipApiModel>> GetDevices(string token)
    {
        var user = await userService.GetUserByTokenAsync(token)
            ?? throw new InvalidDataException(nameof(token));

        // Get all devices from user's projects
        var userProjectIds = await context.UserProjects
            .Where(up => up.UserId == user.Id)
            .Select(up => up.ProjectId)
            .ToListAsync();

        var devices = await context.Devices
            .AsNoTracking()
            .Include(d => d.CurrentSoftware)
            .Include(d => d.Group)
            .Where(d => userProjectIds.Contains(d.Group.ProjectId) && d.CurrentSoftware != null && !d.IsDeleted && !d.Group.IsDeleted)
            .ToListAsync();

        // Get device IDs to fetch last activity
        var deviceIds = devices.Select(d => d.Id).ToList();

        // Get last activity for each device
        var lastActivities = await context.DeviceActivities
            .AsNoTracking()
            .Where(da => deviceIds.Contains(da.DeviceId))
            .GroupBy(da => da.DeviceId)
            .Select(g => new
            {
                DeviceId = g.Key,
                LastActivity = g.Max(da => da.Timestamp)
            })
            .ToDictionaryAsync(x => x.DeviceId, x => (DateTime?)x.LastActivity);

        return devices.Select(d => new ChipApiModel
        {
            MacAdress = d.MacAddress,
            DeviceId = d.DeviceId,
            Software = d.CurrentSoftware!.Name,
            SoftwareVersion = d.CurrentSoftware!.VerMajor.ToString(),
            ChipType = d.DeviceId, // Using DeviceId as chip type identifier
            GroupName = d.Group.Name,
            LastActivity = lastActivities.ContainsKey(d.Id) ? lastActivities[d.Id] : null
        }).ToList();
    }

    public async Task<IEnumerable<Device>> GetGroupDevicesAsync(Guid groupId, string token)
    {
        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == groupId)
            ?? throw new InvalidOperationException("Group not found");

        // Verify user has access to project
        await projectService.GetProjectByIdAsync(group.ProjectId, token);

        return await context.Devices
            .AsNoTracking()
            .Include(d => d.CurrentSoftware)
            .Where(d => d.GroupId == groupId && !d.IsDeleted)
            .ToListAsync();
    }

    public async Task<Device?> GetDeviceByDeviceIdAsync(string deviceId)
    {
        return await context.Devices
            .AsNoTracking()
            .Include(d => d.Group)
            .Include(d => d.CurrentSoftware)
            .FirstOrDefaultAsync(d => d.DeviceId == deviceId && !d.IsDeleted);
    }

    public async Task AssignDeviceToGroupAsync(string deviceId, Guid groupId, string token)
    {
        var device = await context.Devices.FirstOrDefaultAsync(d => d.DeviceId == deviceId)
            ?? throw new InvalidOperationException("Device not found");

        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == groupId)
            ?? throw new InvalidOperationException("Group not found");

        // Verify user has access to project
        await projectService.GetProjectByIdAsync(group.ProjectId, token);

        device.GroupId = groupId;
        device.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task<object?> GetDeviceDetailAsync(Guid deviceId, string token)
    {
        var device = await context.Devices
            .AsNoTracking()
            .Include(d => d.Group)
            .ThenInclude(g => g.Project)
            .Include(d => d.CurrentSoftware)
            .Include(d => d.PendingSoftware)
            .FirstOrDefaultAsync(d => d.Id == deviceId && !d.IsDeleted);

        if (device == null)
            return null;

        // Verify user has access to project
        await projectService.GetProjectByIdAsync(device.Group.ProjectId, token);

        // Get last activity
        var lastActivity = await context.DeviceActivities
            .AsNoTracking()
            .Where(da => da.DeviceId == deviceId)
            .OrderByDescending(da => da.Timestamp)
            .FirstOrDefaultAsync();

        return new
        {
            device.Id,
            device.DeviceId,
            device.MacAddress,
            device.GroupId,
            GroupName = device.Group.Name,
            ProjectId = device.Group.ProjectId,
            ProjectName = device.Group.Project.Name,
            CurrentSoftware = device.CurrentSoftware == null ? null : new
            {
                device.CurrentSoftware.Id,
                device.CurrentSoftware.Name,
                device.CurrentSoftware.VerMajor
            },
            PendingSoftware = device.PendingSoftware == null ? null : new
            {
                device.PendingSoftware.Id,
                device.PendingSoftware.Name,
                device.PendingSoftware.VerMajor
            },
            LastActivity = lastActivity?.Timestamp,
            device.CreatedAt,
            device.UpdatedAt
        };
    }
}
