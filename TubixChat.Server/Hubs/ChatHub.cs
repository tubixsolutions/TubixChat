using Microsoft.AspNetCore.SignalR;

namespace TubixChat.Server.Hubs;

public class ChatHub : Hub
{
    // UserId va ConnectionId ni saqlash
    private static readonly Dictionary<int, string> _userConnections = new();
    private static readonly object _lock = new();

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");

        // User disconnect bo'lganda
        lock (_lock)
        {
            var userId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId != 0)
            {
                _userConnections.Remove(userId);
                // Barcha clientlarga offline xabar yuborish
                Clients.All.SendAsync("UserStatusChanged", userId, false);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    // User connect bo'lganda
    public async Task Connect(int userId)
    {
        lock (_lock)
        {
            _userConnections[userId] = Context.ConnectionId;
        }

        Console.WriteLine($"User {userId} connected with ConnectionId: {Context.ConnectionId}");

        // Barcha clientlarga online xabar yuborish
        await Clients.All.SendAsync("UserStatusChanged", userId, true);
    }

    // Xabar yuborish
    public async Task SendMessage(int senderId, int receiverId, string messageText, long messageId)
    {
        Console.WriteLine($"SendMessage: From {senderId} to {receiverId} - {messageText}");

        var message = new
        {
            Id = messageId,
            SenderUserId = senderId,
            RecieverUserId = receiverId,
            MessageText = messageText,
            IsRead = false,
            IsDelivered = true,
            CreatedAt = DateTime.UtcNow,
            IsMine = false
        };

        // Qabul qiluvchiga xabar yuborish
        if (_userConnections.TryGetValue(receiverId, out var receiverConnectionId))
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", message);
            Console.WriteLine($"Message sent to receiver {receiverId}");
        }
        else
        {
            Console.WriteLine($"Receiver {receiverId} is offline");
        }

        // Yuboruvchiga delivery confirmation
        if (_userConnections.TryGetValue(senderId, out var senderConnectionId))
        {
            await Clients.Client(senderConnectionId).SendAsync("MessageDelivered", messageId);
        }
    }

    // Xabar o'qildi
    public async Task MarkAsRead(int senderId, long messageId)
    {
        Console.WriteLine($"MarkAsRead: MessageId {messageId} by sender {senderId}");

        if (_userConnections.TryGetValue(senderId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("MessageRead", messageId);
        }
    }

    // Typing indicator
    public async Task UserTyping(int senderId, int receiverId, bool isTyping)
    {
        if (_userConnections.TryGetValue(receiverId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("UserTyping", senderId, isTyping);
        }
    }

    // Online userlar ro'yxatini olish
    public async Task<List<int>> GetOnlineUsers()
    {
        lock (_lock)
        {
            return _userConnections.Keys.ToList();
        }
    }
}
