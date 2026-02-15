namespace Updater.ApiService.Database.Models;

public class UserProject
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
}
