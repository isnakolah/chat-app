using System.Linq.Expressions;
using Api.Models;
using Shared.DTOs;

namespace Api.Data;

internal interface IChatRepository
{
    Task<IEnumerable<Chat>> GetAllAsync();
    Task<IEnumerable<ChatGetDTO>> GetAllAsync(Expression<Func<Chat, bool>> expression);
    Task AddAsync(Chat chat);
}