using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TubixChat.BizLogicLayer.DTOs;
using TubixChat.BizLogicLayer.Services;
using TubixChat.Desktop.Services;
using TubixChat.Desktop.ViewModels;

namespace TubixChat.Desktop.Views
{
    public partial class ChatWindow : Window
    {
        private readonly IServiceScope _scope;
        private readonly ChatViewModel _viewModel;
        private readonly ISignalRService _signalRService;

        public ChatWindow(UserDto currentUser)
        {
            InitializeComponent();

            // YANGI SCOPE
            _scope = App.ServiceProvider.CreateScope();

            var chatService = _scope.ServiceProvider
                .GetRequiredService<IChatService>();

            // SignalR service ni olish (Singleton, scope'dan emas)
            _signalRService = App.ServiceProvider.GetRequiredService<ISignalRService>();

            _viewModel = new ChatViewModel(chatService, _signalRService, currentUser);

            DataContext = _viewModel;

            _viewModel.Messages.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessagesScrollViewer.ScrollToEnd();
                    });
                }
            };

            // SignalR ga ulanish (login bo'lganda)
            Loaded += async (s, e) =>
            {
                await InitializeSignalRAsync(currentUser);
                await _viewModel.InitializeAsync();
            };

            // Window yopilganda SignalR disconnect va DbContext dispose bo'ladi
            Closed += async (_, __) =>
            {
                await _signalRService.DisconnectAsync();
                _scope.Dispose();
            };
        }

        private async System.Threading.Tasks.Task InitializeSignalRAsync(UserDto currentUser)
        {
            const string signalRUrl = "http://localhost:5000/chatHub";
            await _signalRService.ConnectAsync(signalRUrl, currentUser.Id);
        }
    }
}
