using Api.Models;
using Shared.DTOs;

namespace Api.Common;

internal static class ChatCreateDTOExtensions
{
    public static Chat ToChat(this ChatCreateDTO chatCreateDto)
    {
        return new Chat
        {
            Message = chatCreateDto.Message, 
            Session = chatCreateDto.Session, 
            User = chatCreateDto.User
        };
    }
}