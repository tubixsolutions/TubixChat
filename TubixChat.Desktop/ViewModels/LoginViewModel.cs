using System.Threading.Tasks;
using System.Windows;
using TubixChat.BizLogicLayer.DTOs;
using TubixChat.BizLogicLayer.Services;

namespace TubixChat.Desktop.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin());
            RegisterCommand = new RelayCommand(_ => OnRegisterRequested?.Invoke());
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public RelayCommand LoginCommand { get; }
        public RelayCommand RegisterCommand { get; }

        public event Action? OnLoginSuccess;
        public event Action? OnRegisterRequested;
        public UserDto? CurrentUser { get; private set; }

        private bool CanLogin() => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);

        private async Task LoginAsync()
        {
            ErrorMessage = string.Empty;

            var loginDto = new LoginDto
            {
                UserName = UserName,
                Password = Password
            };

            var result = await _authService.LoginAsync(loginDto);

            if (result.Success)
            {
                CurrentUser = result.User;
                OnLoginSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = result.Message;
            }
        }
    }
}
