using TubixChat.BizLogicLayer.DTOs;

namespace TubixChat.Desktop.Services;

public interface IApiService
{
    Task<(bool Success, string Message, UserDto? User)> RegisterAsync(RegisterDto dto);
    Task<(bool Success, string Message, UserDto? User)> LoginAsync(LoginDto dto);
}
