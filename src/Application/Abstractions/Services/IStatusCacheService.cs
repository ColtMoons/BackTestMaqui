namespace Application.Abstractions.Services;

public interface IStatusCacheService
{
    Task<string> GetStatusNameAsync(int status, CancellationToken ct = default);
}
