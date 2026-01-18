using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using TubixChat.BizLogicLayer.DTOs;

namespace TubixChat.Desktop.Services
{
    public class SignalRService : ISignalRService
    {
        private HubConnection? _hubConnection;
        private int _currentUserId;

        public event Action<MessageDto>? OnMessageReceived;
        public event Action<int, bool>? OnUserStatusChanged;
        public event Action<long>? OnMessageDelivered;
        public event Action<long>? OnMessageRead;
        public event Action<int, bool>? OnUserTyping;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public async Task<bool> ConnectAsync(string serverUrl, int userId)
        {
            try
            {
                _currentUserId = userId;

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(serverUrl)
                    .WithAutomaticReconnect(new[]
                    {
                        TimeSpan.Zero,
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10)
                    })
                    .Build();

                // Reconnection events
                _hubConnection.Reconnecting += (error) =>
                {
                    Console.WriteLine($"SignalR reconnecting: {error?.Message}");
                    return Task.CompletedTask;
                };

                _hubConnection.Reconnected += (connectionId) =>
                {
                    Console.WriteLine($"SignalR reconnected: {connectionId}");
                    return _hubConnection.InvokeAsync("Connect", _currentUserId);
                };

                _hubConnection.Closed += (error) =>
                {
                    Console.WriteLine($"SignalR connection closed: {error?.Message}");
                    return Task.CompletedTask;
                };

                // Event handlerlarni sozlash
                SetupEventHandlers();

                // Ulanish
                await _hubConnection.StartAsync();
                Console.WriteLine("SignalR connected successfully");

                // Serverga userId ni yuborish
                await _hubConnection.InvokeAsync("Connect", userId);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR connection error: {ex.Message}");
                return false;
            }
        }

        private void SetupEventHandlers()
        {
            if (_hubConnection == null) return;

            // ReceiveMessage event
            _hubConnection.On<object>("ReceiveMessage", (messageObj) =>
            {
                try
                {
                    var message = ConvertToMessageDto(messageObj);
                    OnMessageReceived?.Invoke(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling ReceiveMessage: {ex.Message}");
                }
            });

            // UserStatusChanged event
            _hubConnection.On<int, bool>("UserStatusChanged", (userId, isOnline) =>
            {
                OnUserStatusChanged?.Invoke(userId, isOnline);
            });

            // MessageDelivered event
            _hubConnection.On<long>("MessageDelivered", (messageId) =>
            {
                OnMessageDelivered?.Invoke(messageId);
            });

            // MessageRead event
            _hubConnection.On<long>("MessageRead", (messageId) =>
            {
                OnMessageRead?.Invoke(messageId);
            });

            // UserTyping event
            _hubConnection.On<int, bool>("UserTyping", (senderId, isTyping) =>
            {
                OnUserTyping?.Invoke(senderId, isTyping);
            });
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.StopAsync();
                    await _hubConnection.DisposeAsync();
                    Console.WriteLine("SignalR disconnected");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disconnecting: {ex.Message}");
                }
            }
        }

        public async Task SendMessageAsync(int receiverId, string messageText, long messageId)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.InvokeAsync("SendMessage",
                        _currentUserId, receiverId, messageText, messageId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message: {ex.Message}");
                }
            }
        }

        public async Task MarkAsReadAsync(int senderId, long messageId)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.InvokeAsync("MarkAsRead", senderId, messageId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error marking as read: {ex.Message}");
                }
            }
        }

        public async Task SendTypingAsync(int receiverId, bool isTyping)
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.InvokeAsync("UserTyping",
                        _currentUserId, receiverId, isTyping);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending typing: {ex.Message}");
                }
            }
        }

        private MessageDto ConvertToMessageDto(object message)
        {
            var type = message.GetType();

            var id = GetPropertyValue<long>(type, message, "Id");
            var senderUserId = GetPropertyValue<int>(type, message, "SenderUserId");
            var recieverUserId = GetPropertyValue<int>(type, message, "RecieverUserId");
            var messageText = GetPropertyValue<string>(type, message, "MessageText") ?? "";
            var isRead = GetPropertyValue<bool>(type, message, "IsRead");
            var isDelivered = GetPropertyValue<bool>(type, message, "IsDelivered");
            var createdAt = GetPropertyValue<DateTime>(type, message, "CreatedAt");
            var isMine = GetPropertyValue<bool>(type, message, "IsMine");

            return new MessageDto
            {
                Id = id,
                SenderUserId = senderUserId,
                RecieverUserId = recieverUserId,
                MessageText = messageText,
                IsRead = isRead,
                IsDelivered = isDelivered,
                CreatedAt = createdAt,
                IsMine = isMine
            };
        }

        private T GetPropertyValue<T>(Type type, object obj, string propertyName)
        {
            try
            {
                var prop = type.GetProperty(propertyName);
                if (prop != null)
                {
                    var value = prop.GetValue(obj);
                    if (value != null)
                        return (T)value;
                }
            }
            catch { }

            return default(T)!;
        }
    }
}
