namespace Updater.ApiService.Database.Models;

public class DeviceActivity
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? AdditionalInfo { get; set; }

    // Navigation properties
    public virtual Device Device { get; set; } = null!;
}
