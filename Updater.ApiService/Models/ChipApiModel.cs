namespace Updater.ApiService.Models;

public record ChipApiModel
{
    public string MacAdress { get; set; }
    public string DeviceId { get; set; }
    public string Software {  get; set; }
    public string SoftwareVersion { get; set; }
    public string ChipType { get; set; }
    public string GroupName { get; set; }
    public DateTime? LastActivity { get; set; }
}
