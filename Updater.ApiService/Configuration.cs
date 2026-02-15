namespace Updater.ApiService;

public class Configuration
{
    public Configuration(IConfiguration configuration)
    {
        configuration.Bind(this);
    }

    public string MasterToken { get; set; } = string.Empty;
}
