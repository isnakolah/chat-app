using System.Linq.Expressions;
using Api.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Data;

internal class CachedChatRepository : IChatRepository
{
    private const string CHAT_CACHE_KEY = "chats";
    private readonly List<string> CacheKeys = new() { CHAT_CACHE_KEY };

    private readonly ChatRepository _repository;
    private readonly IMemoryCache _cache;

    public CachedChatRepository(IMemoryCache cache, ChatRepository repository)
    {
        (_cache, _repository) = (cache, repository);
    }

    public async Task<IEnumerable<Chat>> GetAllAsync()
    {
        return await _cache.GetOrCreateAsync(CHAT_CACHE_KEY, async _ => await _repository.GetAllAsync());
    }

    public async Task<IEnumerable<Chat>> GetAllAsync(Expression<Func<Chat, bool>> expression)
    {
        return await _repository.GetAllAsync(expression);
    }

    public async Task AddAsync(Chat chat)
    {
        await _repository.AddAsync(chat);

        ClearCache();
    }

    private void ClearCache()
    {
        CacheKeys.ForEach(key => _cache.Remove(key));

        CacheKeys.Clear();
    }
}
