namespace Updater.ApiService.Cache;

public class UserCache
{
    public Dictionary<string, string> cache = [];

    public bool ExistUser(string token)
    {
        return cache.ContainsKey(token);
    }

    public void AddToCache(string token, string nid)
    {
        cache.TryAdd(token, nid);
    }

    public void RemoveFromCache(string token)
    {
        cache.Remove(token);
    }

}
