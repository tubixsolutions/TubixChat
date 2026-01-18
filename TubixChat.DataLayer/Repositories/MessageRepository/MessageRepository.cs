using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TubixChat.DataLayer.Context;
using TubixChat.DataLayer.Entities;

namespace TubixChat.DataLayer.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(EfCoreContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Message>> GetConversationMessagesAsync(int userId1, int userId2, int skip = 0, int take = 50)
        {
            return await _dbSet
                .Include(m => m.Sender)
                .Include(m => m.Reciever)
                .Where(m => (m.SenderUserId == userId1 && m.RecieverUserId == userId2) ||
                           (m.SenderUserId == userId2 && m.RecieverUserId == userId1))
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(int userId)
        {
            return await _dbSet
                .Include(m => m.Sender)
                .Where(m => m.RecieverUserId == userId && !m.IsRead)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(long messageId)
        {
            var message = await _dbSet.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAsReadBySenderAsync(int currentUserId, int senderUserId)
        {
            var messages = await _dbSet
                .Where(m => m.RecieverUserId == currentUserId &&
                           m.SenderUserId == senderUserId &&
                           !m.IsRead)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId, int fromUserId)
        {
            return await _dbSet
                .CountAsync(m => m.RecieverUserId == userId &&
                                m.SenderUserId == fromUserId &&
                                !m.IsRead);
        }
    }
}