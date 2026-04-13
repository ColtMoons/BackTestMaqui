using Application.Abstractions.Services;
using LazyCache;

namespace Infrastructure.Cache;

public class StatusCacheService(IAppCache cache) : IStatusCacheService
{
    private const string CacheKey = "product:statuses";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    
    private static readonly Dictionary<int, string> Statuses = new()
    {
        { 1, "Active"   },
        { 0, "Inactive" }
    };
    
    public async Task<string> GetStatusNameAsync(int status, CancellationToken ct = default)
    {
        Dictionary<int, string>? statuses = await cache.GetOrAddAsync(
            CacheKey,
            () => Task.FromResult(Statuses),
            CacheDuration);

        return statuses.TryGetValue(status, out string? name)
            ? name
            : "Unknown";
    }
}
