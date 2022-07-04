using System.Linq.Expressions;
using Api.Models;

namespace Api.Data;

internal interface IChatRepository
{
    Task<IEnumerable<Chat>> GetAllAsync();
    Task<IEnumerable<Chat>> GetAllAsync(Expression<Func<Chat, bool>> expression);
    Task AddAsync(Chat chat);
}