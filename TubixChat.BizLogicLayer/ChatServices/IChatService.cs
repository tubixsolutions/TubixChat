using System.Collections.Generic;
using System.Threading.Tasks;
using TubixChat.BizLogicLayer.DTOs;

namespace TubixChat.BizLogicLayer.Services;

public interface IChatService
{
    Task<IEnumerable<UserDto>> SearchUsersAsync(string searchText);
    Task<MessageDto> SendMessageAsync(SendMessageDto sendMessageDto);
    Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(int currentUserId, int otherUserId, int skip = 0);
    Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(int userId);
    Task MarkMessagesAsReadAsync(int currentUserId, int senderUserId);
    Task<int> GetUnreadCountAsync(int userId, int fromUserId);
}
