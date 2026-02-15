namespace Updater.ApiService.Cache;

public class SoftwareCache
{
    readonly List<SoftwareCacheComponent> softwareCacheComponents = [];
    public bool IsSoftwareActual(string chipId, string swName, string token, string platform)
    {
        return softwareCacheComponents.Any(x => x.SwName == swName && x.ChipId == chipId && x.Token == token && x.Platform == platform);
    }

    public void AddToCache(string chipId, string swName, string token, string platform)
    {
        if(!softwareCacheComponents.Any(x => x.SwName == swName && x.ChipId == chipId && x.Token == token && x.Platform == platform))
        {
            softwareCacheComponents.Add(new SoftwareCacheComponent(chipId, swName, token, platform));
        }
    }

    public void RemoveFromCache(string swName, string token, string platform)
    {
        var cc = softwareCacheComponents.Where(x => x.SwName == swName && x.Token == token && x.Platform == platform).ToList();

        foreach (var component in cc)
        {
            softwareCacheComponents.Remove(component);
        }
    }
    private record SoftwareCacheComponent(string ChipId, string SwName, string Token, string Platform);

    public Dictionary<string, string> TempChallenges = [];

}

