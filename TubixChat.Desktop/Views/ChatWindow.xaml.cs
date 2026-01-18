using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TubixChat.BizLogicLayer.DTOs;
using TubixChat.BizLogicLayer.Services;
using TubixChat.Desktop.ViewModels;

namespace TubixChat.Desktop.Views
{
    public partial class ChatWindow : Window
    {
        private readonly IServiceScope _scope;
        private readonly ChatViewModel _viewModel;

        public ChatWindow(UserDto currentUser)
        {
            InitializeComponent();

            // YANGI SCOPE
            _scope = App.ServiceProvider.CreateScope();

            var chatService = _scope.ServiceProvider
                .GetRequiredService<IChatService>();

            _viewModel = new ChatViewModel(chatService, currentUser);

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

            // Window yopilganda DbContext dispose bo‘ladi
            Closed += (_, __) => _scope.Dispose();
        }
    }
}
