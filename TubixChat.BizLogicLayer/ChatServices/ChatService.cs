using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TubixChat.BizLogicLayer.DTOs;
using TubixChat.DataLayer.Entities;
using TubixChat.DataLayer.Repositories;

namespace TubixChat.BizLogicLayer.Services;

public class ChatService : IChatService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

    public ChatService(IMessageRepository messageRepository, IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchText)
    {
        searchText = searchText.ToLower();
        var users = await _userRepository.SearchUsersAsync(searchText);
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            IsOnline = u.IsOnline,
            LastSeen = u.LastSeen
        });
    }

    public async Task<MessageDto> SendMessageAsync(SendMessageDto sendMessageDto)
    {
        var message = new Message
        {
            SenderUserId = sendMessageDto.SenderUserId,
            RecieverUserId = sendMessageDto.RecieverUserId,
            MessageText = sendMessageDto.MessageText,
            IsRead = false,
            CreatedAt = DateTime.UtcNow.AddHours(5),
            CreatedUserId = sendMessageDto.SenderUserId
        };

        await _messageRepository.AddAsync(message);

        var sender = await _userRepository.GetByIdAsync(message.SenderUserId);

        return new MessageDto
        {
            Id = message.Id,
            SenderUserId = message.SenderUserId,
            SenderUserName = sender?.UserName ?? "",
            SenderFullName = sender?.FullName ?? "",
            RecieverUserId = message.RecieverUserId,
            MessageText = message.MessageText,
            IsRead = message.IsRead,
            IsDelivered = true,
            CreatedAt = message.CreatedAt,
            IsMine = true
        };
    }

    public async Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(int currentUserId, int otherUserId, int skip = 0)
    {
        var messages = await _messageRepository.GetConversationMessagesAsync(currentUserId, otherUserId, skip, 50);

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderUserId = m.SenderUserId,
            SenderUserName = m.Sender?.UserName ?? "",
            SenderFullName = m.Sender?.FullName ?? "",
            RecieverUserId = m.RecieverUserId,
            RecieverUserName = m.Reciever?.UserName ?? "",
            MessageText = m.MessageText,
            IsPinned = m.IsPinned,
            IsRead = m.IsRead,
            IsDelivered = true,
            CreatedAt = m.CreatedAt,
            IsMine = m.SenderUserId == currentUserId
        }).OrderBy(m => m.CreatedAt);
    }

    public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(int userId)
    {
        var allMessages = await _messageRepository.FindAsync(m => m.SenderUserId == userId || m.RecieverUserId == userId);

        if (!allMessages.Any())
            return new List<ConversationDto>();

        var conversations = allMessages
            .GroupBy(m => m.SenderUserId == userId ? m.RecieverUserId : m.SenderUserId)
            .Select(g => new
            {
                UserId = g.Key,
                LastMessage = g.OrderByDescending(m => m.CreatedAt).First()
            })
            .ToList();

        var result = new List<ConversationDto>();
        
        foreach (var conv in conversations)
        {
            var user = await _userRepository.GetByIdAsync(conv.UserId);
            if (user == null) continue;

            var unreadCount = await _messageRepository.GetUnreadCountAsync(userId, conv.UserId);

            result.Add(new ConversationDto
            {
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    IsOnline = user.IsOnline,
                    LastSeen = user.LastSeen
                },
                LastMessage = new MessageDto
                {
                    MessageText = conv.LastMessage.MessageText,
                    CreatedAt = conv.LastMessage.CreatedAt,
                    IsRead = conv.LastMessage.IsRead
                },
                UnreadCount = unreadCount
            });
        }

        return result.OrderByDescending(c => c.LastMessage?.CreatedAt);
    }

    public async Task MarkMessagesAsReadAsync(int currentUserId, int senderUserId)
    {
        await _messageRepository.MarkAsReadBySenderAsync(currentUserId, senderUserId);
    }

    public async Task<int> GetUnreadCountAsync(int userId, int fromUserId)
    {
        return await _messageRepository.GetUnreadCountAsync(userId, fromUserId);
    }
}
