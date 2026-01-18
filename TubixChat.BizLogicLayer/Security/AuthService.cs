using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TubixChat.BizLogicLayer.DTOs;
using TubixChat.DataLayer.Entities;
using TubixChat.DataLayer.Repositories;

namespace TubixChat.BizLogicLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<(bool Success, string Message, UserDto? User)> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userRepository.UserExistsAsync(registerDto.UserName.ToLower()))
            {
                return (false, "Bu username allaqachon band!", null);
            }

            var (hash, salt) = CreatePasswordHash(registerDto.Password);

            var user = new User
            {
                UserName = registerDto.UserName,
                PasswordHash = hash,
                PasswordSalt = salt,
                FullName = registerDto.FullName,
                PhoneNumber = registerDto.PhoneNumber,
                StateId = 1, // Aktiv
                CreatedAt = DateTime.UtcNow.AddHours(5) // UTC+5
            };

            await _userRepository.AddAsync(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                IsOnline = false
            };

            return (true, "Ro'yxatdan muvaffaqiyatli o'tdingiz!", userDto);
        }

        public async Task<(bool Success, string Message, UserDto? User)> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUserNameAsync(loginDto.UserName);

            if (user == null)
            {
                return (false, "Foydalanuvchi topilmadi!", null);
            }

            if (!VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return (false, "Username yoki parol noto'g'ri!", null);
            }

            if (user.StateId != 1)
            {
                return (false, "Hisobingiz bloklangan!", null);
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                IsOnline = true
            };

            return (true, "Tizimga muvaffaqiyatli kirdingiz!", userDto);
        }

        private (string Hash, string Salt) CreatePasswordHash(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = Convert.ToBase64String(hmac.Key);
            var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return (hash, salt);
        }

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            using var hmac = new HMACSHA512(saltBytes);
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return computedHash == storedHash;
        }
    }
}