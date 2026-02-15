namespace Updater.ApiService.Database.Models;

public class Device
{
    public Guid Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public Guid GroupId { get; set; }
    public Guid? CurrentSoftwareId { get; set; }
    public Guid? PendingSoftwareId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual Group Group { get; set; } = null!;
    public virtual Software? CurrentSoftware { get; set; }
    public virtual Software? PendingSoftware { get; set; }
}
