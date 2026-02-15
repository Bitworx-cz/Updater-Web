namespace Updater.ApiService.Database.Models;

public class Group
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? TargetSoftwareId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool UseHMAC { get; set; } = false;
    public string HMACKey { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual Software? TargetSoftware { get; set; }
    public virtual ICollection<Software> Softwares { get; set; } = new List<Software>();
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
