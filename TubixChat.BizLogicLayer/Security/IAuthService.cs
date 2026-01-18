using System.Threading.Tasks;
using TubixChat.BizLogicLayer.DTOs;

namespace TubixChat.BizLogicLayer.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, UserDto? User)> RegisterAsync(RegisterDto registerDto);
    Task<(bool Success, string Message, UserDto? User)> LoginAsync(LoginDto loginDto);
}
