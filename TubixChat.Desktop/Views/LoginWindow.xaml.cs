using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using TubixChat.Desktop.ViewModels;

namespace TubixChat.Desktop.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.OnLoginSuccess += () =>
            {
                var chatWindow = new ChatWindow(_viewModel.CurrentUser!);
                chatWindow.Show();
                this.Close();
            };

            _viewModel.OnRegisterRequested += () =>
            {
                var registerWindow = new RegisterWindow();
                registerWindow.Show();
                this.Close();
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
