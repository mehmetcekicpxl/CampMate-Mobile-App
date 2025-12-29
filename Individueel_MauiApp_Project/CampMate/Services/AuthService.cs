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
                Console.WriteLine($"Login Fout: {ex.Message}");
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
                Console.WriteLine($"Register Fout: {ex.Message}");
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


        // Methode om gebruikersprofiel bij te werken
        public async Task<bool> UpdateUserProfileAsync(int userId, UserUpdateDto updateData)
        {
            try
            {
                // 1. Stuur een PUT-verzoek naar de API (Database update)
                var response = await _httpClient.PutAsJsonAsync($"auth/update/{userId}", updateData);

                if (response.IsSuccessStatusCode)
                {
                    

                   
                    // We halen de huidige opgeslagen gebruiker op.
                    
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

                            // De bijgewerkte gebruiker terug opslaan in SecureStorage  de app de nieuwe gegevens direct ziet zonder uit te loggen.

                            var newJson = JsonSerializer.Serialize(userDto);
                            await SecureStorage.SetAsync("CurrentUser", newJson);
                        }
                    }

                    return true;
                }
                else
                {
                    // Foutmelding lezen
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Update fout: {errorMsg}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Er is een fout opgetreden tijdens het updaten: {ex.Message}");
                return false;
            }
        }


        // Methode om een reactie te posten
        public async Task<bool> AddCommentAsync(int campId, string userName, string text)
        {
            try
            {
                var commentDto = new CreateCommentDto
                {
                    CampSiteId = campId,
                    UserName = userName,
                    Text = text
                };

                // Stuur POST request naar de API
                var response = await _httpClient.PostAsJsonAsync("campsites/comments", commentDto);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fout bij het plaatsen van reactie: {ex.Message}");
                return false;
            }
        }


        // Methode om foto te uploaden
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
    }
}