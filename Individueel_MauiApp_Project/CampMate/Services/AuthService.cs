using CampMate.Shared.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace CampMate.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDto?> LoginAsync(string email, string password)
        {
            try
            {
                var loginDto = new LoginDto { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync("auth/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDto>();

                    // Als de aanmelding succesvol is, slaan we de gebruiker op in het geheugen.
                    if (user != null)
                    {
                        string userJson = JsonSerializer.Serialize(user);
                        await SecureStorage.SetAsync("CurrentUser", userJson);
                    }
                    return user;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login Hatası: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/register", registerDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register Hatası: {ex.Message}");
                return false;
            }
        }


        // Haal de gegevens van de huidige gebruiker op uit Storage.
        public async Task<UserDto?> GetCurrentUserAsync()
        {
            try
            {
                string userJson = await SecureStorage.GetAsync("CurrentUser");
                if (!string.IsNullOrEmpty(userJson))
                {
                    return JsonSerializer.Deserialize<UserDto>(userJson);
                }
            }
            catch { }
            return null;
        }

        
        public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            try
            {
                var dto = new ChangePasswordDto
                {
                    Email = email,
                    OldPassword = oldPassword,
                    NewPassword = newPassword
                };

                var response = await _httpClient.PostAsJsonAsync("auth/change-password", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            SecureStorage.Remove("CurrentUser");
        }
    }
}