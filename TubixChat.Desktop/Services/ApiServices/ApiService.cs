using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TubixChat.BizLogicLayer.DTOs;

namespace TubixChat.Desktop.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private const string BASE_URL = "http://45.130.148.91:5000/api";

    public ApiService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BASE_URL) };
    }

    public async Task<(bool Success, string Message, UserDto? User)> RegisterAsync(RegisterDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/auth/register", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return (result?.Success ?? false, result?.Message ?? "", result?.User);
            }

            return (false, "Server bilan bog'lanishda xatolik", null);
        }
        catch (Exception ex)
        {
            return (false, $"Xatolik: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, UserDto? User)> LoginAsync(LoginDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/auth/login", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                return (result?.Success ?? false, result?.Message ?? "", result?.User);
            }

            return (false, "Login yoki parol noto'g'ri", null);
        }
        catch (Exception ex)
        {
            return (false, $"Xatolik: {ex.Message}", null);
        }
    }

    private class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public UserDto? User { get; set; }
    }
}
