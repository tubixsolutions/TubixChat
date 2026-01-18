using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TubixChat.BizLogicLayer.DTOs;
using TubixChat.BizLogicLayer.Services;
using TubixChat.Desktop.Services;

namespace TubixChat.Desktop.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly IChatService _chatService;
        private readonly ISignalRService _signalRService;
        private readonly UserDto _currentUser;

        private UserDto? _selectedUser;
        private string _messageText = string.Empty;
        private string _searchText = string.Empty;

        public ChatViewModel(IChatService chatService, ISignalRService signalRService, UserDto currentUser)
        {
            _chatService = chatService;
            _signalRService = signalRService;
            _currentUser = currentUser;

            Conversations = new ObservableCollection<ConversationDto>();
            Messages = new ObservableCollection<MessageDto>();
            SearchResults = new ObservableCollection<UserDto>();

            SendMessageCommand = new RelayCommand(async _ => await SendMessageAsync(), _ => CanSendMessage());
            SearchUsersCommand = new RelayCommand(async _ => await SearchUsersAsync());
            SelectUserCommand = new RelayCommand(async param => await SelectUserAsync(param as UserDto));

            // SignalR event'larni subscribe qilish
            _signalRService.OnMessageReceived += OnMessageReceived;
            _signalRService.OnMessageDelivered += OnMessageDelivered;
            _signalRService.OnMessageRead += OnMessageRead;
            _signalRService.OnUserStatusChanged += OnUserStatusChanged;
        }

        public async Task InitializeAsync()
        {
            await LoadConversationsAsync();
        }

        public ObservableCollection<ConversationDto> Conversations { get; }
        public ObservableCollection<MessageDto> Messages { get; }
        public ObservableCollection<UserDto> SearchResults { get; }

        public UserDto? SelectedUser
        {
            get => _selectedUser;
            private set => SetProperty(ref _selectedUser, value);
        }

        public string MessageText
        {
            get => _messageText;
            set => SetProperty(ref _messageText, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string CurrentUserName => _currentUser.FullName;

        public RelayCommand SendMessageCommand { get; }
        public RelayCommand SearchUsersCommand { get; }
        public RelayCommand SelectUserCommand { get; }

        private bool CanSendMessage() => SelectedUser != null && !string.IsNullOrWhiteSpace(MessageText);

        private async Task SendMessageAsync()
        {
            if (SelectedUser == null) return;

            var dto = new SendMessageDto
            {
                SenderUserId = _currentUser.Id,
                RecieverUserId = SelectedUser.Id,
                MessageText = MessageText
            };

            // API orqali xabar yuborish (database ga saqlash uchun)
            var message = await _chatService.SendMessageAsync(dto);

            // SignalR orqali real-time yuborish
            if (_signalRService.IsConnected)
            {
                await _signalRService.SendMessageAsync(SelectedUser.Id, MessageText, message.Id);
            }

            Messages.Add(message);
            MessageText = string.Empty;

            await LoadConversationsAsync();
        }

        private async Task LoadMessagesAsync()
        {
            if (SelectedUser == null) return;

            Messages.Clear();

            var messages = await _chatService.GetConversationMessagesAsync(_currentUser.Id, SelectedUser.Id);

            foreach (var msg in messages)
                Messages.Add(msg);

            await _chatService
                .MarkMessagesAsReadAsync(_currentUser.Id, SelectedUser.Id);
        }

        private async Task LoadConversationsAsync()
        {
            Conversations.Clear();

            var conversations =
                await _chatService.GetUserConversationsAsync(_currentUser.Id);

            foreach (var conv in conversations)
                Conversations.Add(conv);
        }

        private async Task SearchUsersAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SearchResults.Clear();
                return;
            }

            var users = await _chatService.SearchUsersAsync(SearchText);

            SearchResults.Clear();

            foreach (var user in users.Where(u => u.Id != _currentUser.Id))
                SearchResults.Add(user);
        }

        private async Task SelectUserAsync(UserDto? user)
        {
            if (user == null) return;

            SelectedUser = user;

            SearchText = string.Empty;
            SearchResults.Clear();

            await LoadMessagesAsync();
            await LoadConversationsAsync();
        }

        public async Task ReceiveMessageAsync(MessageDto message)
        {
            if (SelectedUser?.Id == message.SenderUserId)
            {
                Messages.Add(message);

                await _chatService.MarkMessagesAsReadAsync(_currentUser.Id, message.SenderUserId);
            }

            await LoadConversationsAsync();
        }

        // SignalR event handler'lar
        private async void OnMessageReceived(MessageDto message)
        {
            // Agar hozirgi tanlangan user dan kelayotgan bo'lsa, ko'rsatish
            if (SelectedUser?.Id == message.SenderUserId)
            {
                Messages.Add(message);
                await _chatService.MarkMessagesAsReadAsync(_currentUser.Id, message.SenderUserId);
            }

            // Conversations ro'yxatini yangilash
            await LoadConversationsAsync();
        }

        private void OnMessageDelivered(long messageId)
        {
            // Xabar yetkazilganini ko'rsatish
            var message = Messages.FirstOrDefault(m => m.Id == messageId);
            if (message != null)
            {
                message.IsDelivered = true;
            }
        }

        private void OnMessageRead(long messageId)
        {
            // Xabar o'qilganini ko'rsatish
            var message = Messages.FirstOrDefault(m => m.Id == messageId);
            if (message != null)
            {
                message.IsRead = true;
            }
        }

        private void OnUserStatusChanged(int userId, bool isOnline)
        {
            // User status o'zgarganini yangilash
            // Bu yerda ConversationDto yoki boshqa joyda status ko'rsatilishi mumkin
        }
    }
}
