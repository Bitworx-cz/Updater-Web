using System.Security.Cryptography;
using System.Text;
using Updater.ApiService.Database.Models;
using Updater.ApiService.Database;

namespace Updater.ApiService.Services;

public class Helpers(Context context, ILogger<Helpers> logger)
{
    public static string GenerateToken()
    {
        byte[] randomBytes = new byte[16]; // 16 bytes = 128 bits
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        // Convert to hexadecimal string
        StringBuilder sb = new StringBuilder(32);
        foreach (byte b in randomBytes)
        {
            sb.Append(b.ToString("x2")); // Hex representation
        }

        return sb.ToString(); // 32 characters long
    }

    public async Task LogDeviceActivity(Guid deviceId, string endpoint, string? additionalInfo = null)
    {
        try
        {
            var activity = new DeviceActivity
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                Endpoint = endpoint,
                Timestamp = DateTime.UtcNow,
                AdditionalInfo = additionalInfo
            };

            await context.DeviceActivities.AddAsync(activity);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log but don't fail the request
            logger.LogError(ex, "Failed to log device activity for device {DeviceId} on endpoint {Endpoint}", deviceId, endpoint);
        }
    }
}
