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
                    if (user != null)
                    {
                        string userJson = JsonSerializer.Serialize(user);
                        await SecureStorage.SetAsync("CurrentUser", userJson);

                        // Opslaan van UserId in preferences voor eenvoudig gebruik
                        Preferences.Set("UserId", user.Id);
                        Preferences.Set("IsAdmin", user.IsAdmin);
                    }
                    return user;
                }
            }
            catch { }
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
                Console.WriteLine($"Register Fout: {ex.Message}");
                return false;
            }
        }

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
                var dto = new ChangePasswordDto { Email = email, OldPassword = oldPassword, NewPassword = newPassword };
                var response = await _httpClient.PostAsJsonAsync("auth/change-password", dto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

       
        public async Task<(bool IsSuccess, string ErrorMessage)> UpdateUserProfileAsync(int userId, UserUpdateDto updateData)
        {
            try
            {
               
                var response = await _httpClient.PutAsJsonAsync($"auth/update/{userId}", updateData);

                if (response.IsSuccessStatusCode)
                {
                   

                    // Werk ook de gebruiker in het telefoongeheugen (SecureStorage) bij, zodat hij niet hoeft uit te loggen en weer in te loggen.
                    var json = await SecureStorage.GetAsync("CurrentUser");
                    if (!string.IsNullOrEmpty(json))
                    {
                        var userDto = JsonSerializer.Deserialize<UserDto>(json);
                        if (userDto != null)
                        {
                            userDto.Name = updateData.Name;
                            userDto.Email = updateData.Email;
                            userDto.PhoneNumber = updateData.PhoneNumber;
                            userDto.Location = updateData.Location;

                            
                            if (!string.IsNullOrEmpty(updateData.ProfileImage))
                                userDto.ProfileImage = updateData.ProfileImage;

                            var newJson = JsonSerializer.Serialize(userDto);
                            await SecureStorage.SetAsync("CurrentUser", newJson);
                        }
                    }

                    
                    return (true, null);
                }
                else
                {
                   
                    string errorContent = await response.Content.ReadAsStringAsync();

                   
                    return (false, errorContent);
                }
            }
            catch (Exception ex)
            {
                // Als er geen internetverbinding is of als de server uitgeschakeld is 
                return (false, $"Verbindingsfout: {ex.Message}");
            }
        }

        
        public void Logout()
        {
            SecureStorage.Remove("CurrentUser");
            Preferences.Clear();
        }

      
        public async Task<string?> UploadImageAsync(FileResult fileResult)
        {
            try
            {
                var stream = await fileResult.OpenReadAsync();
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(stream), "file", fileResult.FileName);

                var response = await _httpClient.PostAsync("upload", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    return result.GetProperty("imageName").GetString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload fout: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> AddCommentAsync(int campId, string userName, string text, int rating)
        {
            try
            {
               
                int currentUserId = Preferences.Get("UserId", 0);

                var commentDto = new CreateCommentDto
                {
                    CampSiteId = campId,
                    UserName = userName,
                    Text = text,
                    UserId = currentUserId,
                    Rating = rating
                };

                var response = await _httpClient.PostAsJsonAsync("campsites/comments", commentDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Comment fout: {ex.Message}");
                return false;
            }
        }
    }
}