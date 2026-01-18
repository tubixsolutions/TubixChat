using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TubixChat.Desktop.ViewModels;

namespace TubixChat.Desktop.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly RegisterViewModel _viewModel;

        public RegisterWindow()
        {
            InitializeComponent();
            // Dependency Injection orqali olish kerak
            _viewModel = App.ServiceProvider.GetService<RegisterViewModel>();
            DataContext = _viewModel;

            _viewModel.OnRegisterSuccess += () =>
            {
                MessageBox.Show("Ro'yxatdan muvaffaqiyatli o'tdingiz! Endi login qiling.",
                    "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);
                var loginWindow = new LoginWindow(App.ServiceProvider.GetService<LoginViewModel>());
                loginWindow.Show();
                this.Close();
            };

            _viewModel.OnBackRequested += () =>
            {
                var loginWindow = new LoginWindow(App.ServiceProvider.GetService<LoginViewModel>());
                loginWindow.Show();
                this.Close();
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
        }
    }
}
