namespace Updater.ApiService.Database.Models;

public class Software
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int VerMajor { get; set; }
    public Guid UploadedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual Group Group { get; set; } = null!;
    public virtual User Uploader { get; set; } = null!;
    public virtual Binary? Binary { get; set; }
    public virtual ICollection<Group> GroupsUsingThisSoftware { get; set; } = new List<Group>();
    public virtual ICollection<Device> DevicesUsingThisSoftware { get; set; } = new List<Device>();
}
