using CachingApi.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CachingApi.Services;

public class CacheService : ICacheService
{
    private IDatabase _cacheDb;
    
    public CacheService()
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379");
        _cacheDb = redis.GetDatabase();
    }
    
    public T GetData<T>(string key)
    {
        var value = _cacheDb.StringGet(key);

        // if (!string.IsNullOrEmpty(value)){}
        if (!value.IsNullOrEmpty)
            return JsonConvert.DeserializeObject<T>(value);
        return default;
    }

    public object RemoveData(string key)
    {
        var exists = _cacheDb.KeyExists(key);
        if (exists)
            _cacheDb.KeyDelete(key);

        return false;
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);

        return _cacheDb.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);

    }
}