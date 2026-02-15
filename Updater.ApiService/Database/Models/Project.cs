namespace Updater.ApiService.Database.Models;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
