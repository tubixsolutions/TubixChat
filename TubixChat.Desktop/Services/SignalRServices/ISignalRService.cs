using System;
using System.Threading.Tasks;
using TubixChat.BizLogicLayer.DTOs;

namespace TubixChat.Desktop.Services;

public interface ISignalRService
{
    Task<bool> ConnectAsync(string serverUrl, int userId);
    Task DisconnectAsync();
    Task SendMessageAsync(int receiverId, string messageText, long messageId);
    Task MarkAsReadAsync(int senderId, long messageId);
    Task SendTypingAsync(int receiverId, bool isTyping);

    event Action<MessageDto>? OnMessageReceived;
    event Action<int, bool>? OnUserStatusChanged;
    event Action<long>? OnMessageDelivered;
    event Action<long>? OnMessageRead;
    event Action<int, bool>? OnUserTyping;

    bool IsConnected { get; }
}
