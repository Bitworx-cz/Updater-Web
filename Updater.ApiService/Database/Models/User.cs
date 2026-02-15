namespace Updater.ApiService.Database.Models;

public class User
{
    public Guid Id { get; set; }
    public string Nid { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
    public virtual ICollection<Software> UploadedSoftwares { get; set; } = new List<Software>();
}
