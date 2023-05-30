using ChatGptBot.Ioc;
using ChatGptBot.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ChatGptBot.Repositories;
public interface IEmbeddingRepositoryCache
{
    Task<List<Embedding>> LoadSet(string code);
    
}
public class EmbeddingRepositoryCache : IEmbeddingRepositoryCache,ISingletonScope
{
    private readonly IEmbeddingRepository _embeddingRepository;
    private readonly IMemoryCache _memoryCache;

    public EmbeddingRepositoryCache(IEmbeddingRepository embeddingRepository,IMemoryCache memoryCache)
    {
        _embeddingRepository = embeddingRepository;
        _memoryCache = memoryCache;
    }
    public async Task<List<Embedding>> LoadSet(string code)
    {
        var response = _memoryCache.Get<List<Embedding>>(code);
        if (response == null)
        {
            response = await _embeddingRepository.LoadSet(code);
            _memoryCache.Set(code, response, DateTimeOffset.MaxValue);
        }
        return response;
    }

    
}