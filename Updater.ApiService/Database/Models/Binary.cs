using System.ComponentModel.DataAnnotations;

namespace Updater.ApiService.Database.Models;

public class Binary
{
    [Key]
    public Guid SoftwareId { get; set; }
    public byte[] RawBinary { get; set; } = [];

    // Navigation property
    public virtual Software Software { get; set; } = null!;
}
