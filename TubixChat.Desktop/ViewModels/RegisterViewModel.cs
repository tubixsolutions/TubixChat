using System.Threading.Tasks;
using TubixChat.BizLogicLayer.DTOs;
using TubixChat.BizLogicLayer.Services;

namespace TubixChat.Desktop.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _fullName = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _errorMessage = string.Empty;

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
            RegisterCommand = new RelayCommand(async _ => await RegisterAsync(), _ => CanRegister());
            BackCommand = new RelayCommand(_ => OnBackRequested?.Invoke());
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public RelayCommand RegisterCommand { get; }
        public RelayCommand BackCommand { get; }

        public event Action? OnRegisterSuccess;
        public event Action? OnBackRequested;

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(UserName) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(FullName) &&
                   Password == ConfirmPassword;
        }

        private async Task RegisterAsync()
        {
            ErrorMessage = string.Empty;

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Parollar mos kelmadi!";
                return;
            }

            var registerDto = new RegisterDto
            {
                UserName = UserName,
                Password = Password,
                FullName = FullName,
                PhoneNumber = PhoneNumber
            };

            var result = await _authService.RegisterAsync(registerDto);

            if (result.Success)
            {
                OnRegisterSuccess?.Invoke();
            }
            else
            {
                ErrorMessage = result.Message;
            }
        }
    }
}
