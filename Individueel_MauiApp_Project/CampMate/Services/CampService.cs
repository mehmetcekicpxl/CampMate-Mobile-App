using CampMate.Shared.DTOs;
using System.Net.Http.Json;

namespace CampMate.Services
{
    public class CampService
    {
        private readonly HttpClient _httpClient;

        public CampService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

       
        public async Task<CampSiteDto?> AddCampAsync(CampSiteDto campDto)
        {
            try
            {
               
                var response = await _httpClient.PostAsJsonAsync("campsites", campDto);

                if (response.IsSuccessStatusCode)
                {
                    
                    return await response.Content.ReadFromJsonAsync<CampSiteDto>();
                }
                else
                {
                   
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Fout: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Toevoegen Fout: {ex.Message}");
            }
            return null; 
        }


        
        public async Task<bool> DeleteCampAsync(int campId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"campsites/{campId}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        
        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            try
            {
               
                var response = await _httpClient.DeleteAsync($"campsites/comments/{commentId}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        
        public async Task<CampSiteDto?> GetCampByIdAsync(int id)
        {
            try
            {
                
                return await _httpClient.GetFromJsonAsync<CampSiteDto>($"campsites/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetById Fout: {ex.Message}");
                return null;
            }
        }
    }
}