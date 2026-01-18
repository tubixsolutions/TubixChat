using System.Collections.Generic;
using System.Threading.Tasks;
using TubixChat.DataLayer.Entities;

namespace TubixChat.DataLayer.Repositories;

public interface IMessageRepository : IRepository<Message>
{
    Task<IEnumerable<Message>> GetConversationMessagesAsync(int userId1, int userId2, int skip = 0, int take = 50);
    Task<IEnumerable<Message>> GetUnreadMessagesAsync(int userId);
    Task MarkAsReadAsync(long messageId);
    Task MarkAsReadBySenderAsync(int currentUserId, int senderUserId);
    Task<int> GetUnreadCountAsync(int userId, int fromUserId);
}
