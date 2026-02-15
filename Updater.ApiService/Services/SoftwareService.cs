using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Updater.ApiService.Cache;
using Updater.ApiService.Database;
using Updater.ApiService.Database.Models;

namespace Updater.ApiService.Services;

public class SoftwareService(Context context, ILogger<SoftwareService> logger, UserService userService, SoftwareCache cache, Configuration configuration, ProjectService projectService, GroupService groupService, Helpers helpers)
{
    public async Task<string> UploadSw(string groupToken, string token, string swName, IFormFile sw)
    {
        var group = await context.Groups.FirstOrDefaultAsync(g => g.Token == groupToken)
    ?? throw new InvalidOperationException("Group not found");

        // Verify access to project
        await projectService.GetProjectByIdAsync(group.ProjectId, token);

        var user = await userService.GetUserByTokenAsync(token)
            ?? throw new UnauthorizedAccessException("Invalid user");

        // Get latest version for this software in this group
        var existingSoftware = await context.Softwares
            .AsNoTracking()
            .Where(x => x.GroupId == group.Id && !x.IsDeleted)
            .OrderByDescending(x => x.VerMajor)
            .FirstOrDefaultAsync();

        int nextVersion = existingSoftware != null ? existingSoftware.VerMajor + 1 : 1;

        // Read file
        using var memoryStream = new MemoryStream();
        await sw.CopyToAsync(memoryStream);
        byte[] fileBytes = memoryStream.ToArray();

        // Create new software version
        var newSoftware = new Software
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            Name = swName,
            VerMajor = nextVersion,
            UploadedBy = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        await context.Softwares.AddAsync(newSoftware);

        // Create binary
        var binary = new Binary
        {
            SoftwareId = newSoftware.Id,
            RawBinary = fileBytes
        };

        await context.Binarys.AddAsync(binary);

        // Set as target software for the group
        group.TargetSoftwareId = newSoftware.Id;
        group.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        logger.LogInformation("Uploaded {SoftwareName} v{Version} to group {GroupId}", swName, nextVersion, group.Id);

        return $"{swName} v{nextVersion}";
    }

    public async Task<byte[]> GetSoftware(string deviceId, string token, string? hmac)
    {
        var group = await groupService.GetGroupByTokenAsync(token) ?? throw new UnauthorizedAccessException("Invalid token");

        var device = await context.Devices
            .FirstOrDefaultAsync(d => d.DeviceId == deviceId && d.GroupId == group.Id)
            ?? throw new InvalidOperationException("Device not found");

        if (device != null)
        {
            await helpers.LogDeviceActivity(device.Id, "/device/download", $"GroupId: {group.Id}");
        }

        if (group.UseHMAC && string.IsNullOrEmpty(hmac))
        {
            await helpers.LogDeviceActivity(device.Id, "/device/download", $"GroupId: {group.Id}, HMAC was not set");
            throw new UnauthorizedAccessException("You need HMAC for group with HMAC selected");
        }

        if(!string.IsNullOrEmpty(hmac) && group.UseHMAC && !Hmac(deviceId,hmac, group.HMACKey))
        {
            await helpers.LogDeviceActivity(device.Id, "/device/download", $"GroupId: {group.Id}, HMAC was not correct");
            throw new UnauthorizedAccessException("Incorrect HMAC");
        }

        // Get target software from group
        if (group.TargetSoftwareId == null)
            return [];

        // Set pending software to track what we're sending
        device.PendingSoftwareId = group.TargetSoftwareId;
        device.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        logger.LogInformation("Device {DeviceId} downloading software {SoftwareId}", deviceId, device.PendingSoftwareId);

        var binary = await context.Binarys
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.SoftwareId == group.TargetSoftwareId.Value);

        return binary?.RawBinary ?? [];
    }

    public async Task<bool> IsUpdateAvailable(string deviceId, string token)
    {
        var group = await groupService.GetGroupByTokenAsync(token) ?? throw new UnauthorizedAccessException("Invalid token");


        var device = await context.Devices
            .FirstOrDefaultAsync(d => d.DeviceId == deviceId && d.GroupId == group.Id);

        if (device != null)
        {
            await helpers.LogDeviceActivity(device.Id, "/device/check-update", $"GroupId: {group.Id}");
        }

        if (device == null || group.TargetSoftwareId == null)
            return false;

        // Update available if target differs from current
        return device.CurrentSoftwareId != group.TargetSoftwareId;
    }

    public async Task SetUpdateDone(string deviceId, string token)
    {
        var group = await groupService.GetGroupByTokenAsync(token) ?? throw new UnauthorizedAccessException("Invalid token");

        var device = await context.Devices
            .FirstOrDefaultAsync(d => d.DeviceId == deviceId && d.GroupId == group.Id)
            ?? throw new InvalidOperationException("Device not found");

        await helpers.LogDeviceActivity(device.Id, "/device/update-done", $"GroupId: {group.Id}");


        // Move pending software to current software
        if (device.PendingSoftwareId != null)
        {
            device.CurrentSoftwareId = device.PendingSoftwareId;
            device.PendingSoftwareId = null;
            device.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            logger.LogInformation("Device {DeviceId} updated to software {SoftwareId}", deviceId, device.CurrentSoftwareId);
        }
    }

    public async Task<IEnumerable<Software>> GetGroupSoftwareVersions(Guid groupId, string token)
    {
        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == groupId)
            ?? throw new InvalidOperationException("Group not found");

        await projectService.GetProjectByIdAsync(group.ProjectId, token);

        return await context.Softwares
            .AsNoTracking()
            .Where(s => s.GroupId == groupId && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    internal async Task ClearOldSoftwares(string token)
    {
        if (token != configuration.MasterToken)
            throw new UnauthorizedAccessException("Invalid master token");

        // Find unused software versions
        var unusedSoftwares = await context.Softwares
            .Where(s =>
                // Not a target software for any group
                !context.Groups.Any(g => g.TargetSoftwareId == s.Id) &&
                // Not currently installed on any device
                !context.Devices.Any(d => d.CurrentSoftwareId == s.Id))
            .ToListAsync();

        context.Softwares.RemoveRange(unusedSoftwares);
        await context.SaveChangesAsync();

        // Remove orphaned binaries
        var orphanedBinaries = await context.Binarys
            .Where(b => !context.Softwares.Any(s => s.Id == b.SoftwareId))
            .ToListAsync();

        context.Binarys.RemoveRange(orphanedBinaries);
        await context.SaveChangesAsync();

        logger.LogInformation("Cleaned up {SoftwareCount} unused software versions and {BinaryCount} orphaned binaries",
            unusedSoftwares.Count, orphanedBinaries.Count);
    }

    internal async Task<string> Challenge(string deviceId, string token)
    {
        if(! await IsUpdateAvailable(deviceId, token))
        {
            throw new Exception("Update unavailable");
        }

        byte[] challengeBytes = new byte[16];
        RandomNumberGenerator.Fill(challengeBytes);
        string challenge = Convert.ToBase64String(challengeBytes);

        cache.TempChallenges[deviceId] = challenge;

        return challenge;
    }

    private bool Hmac(string deviceId, string hmacResponse, string groupSecret)
    {
        string challenge = cache.TempChallenges[deviceId];

        byte[] key = Encoding.UTF8.GetBytes(groupSecret);
        byte[] challengeBytes = Encoding.UTF8.GetBytes(challenge);

        using var hmac = new HMACSHA256(key);
        byte[] expectedBytes = hmac.ComputeHash(challengeBytes);
        string expectedHmac = Convert.ToBase64String(expectedBytes);

        cache.TempChallenges.Remove(deviceId);
        return hmacResponse == expectedHmac;
    }
}
