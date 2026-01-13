using CampMate.Models;
using CampMate.Services;
using CampMate.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampMate.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {

        // De basis-URL van  API (voor Android Emulator is dit 10.0.2.2)
        
        private const string BaseUrl = "http://10.0.2.2:5225";

       
        public string ProfileImageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentUser.ProfileImage) || CurrentUser.ProfileImage == "profile.png")
                {
                    return "profile.png"; 
                }

               
                return $"{BaseUrl}/images/{CurrentUser.ProfileImage}";
            }
        }



        private readonly AuthService _authService;

        [ObservableProperty]
        private User _currentUser;

        [ObservableProperty]
        private bool _isEditing;

        public ProfileViewModel(AuthService authService)
        {
            
            _authService = authService;
            CurrentUser = new User();
            IsEditing = false;

           
        }




        public async Task LoadUserDataAsync()
        {
            var userDto = await _authService.GetCurrentUserAsync();

            if (userDto != null)
            {
                // omzetten van DTO naar Model
                CurrentUser = new User
                {
                    Id = userDto.Id,
                    Name = userDto.Name,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    Location = userDto.Location,
                    ProfileImage = userDto.ProfileImage
                };
                OnPropertyChanged(nameof(ProfileImageUrl));
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        [RelayCommand]
        void EditProfile()
        {
            IsEditing = true;
        }


        [RelayCommand]
        async Task SaveChanges()
        {
            if (CurrentUser == null) return;

           
            var updateDto = new UserUpdateDto
            {
                Name = CurrentUser.Name ?? "",
                Email = CurrentUser.Email ?? "",
                PhoneNumber = CurrentUser.PhoneNumber ?? "",
                Location = CurrentUser.Location ?? "",
                ProfileImage = CurrentUser.ProfileImage 
            };

            
            var result = await _authService.UpdateUserProfileAsync(CurrentUser.Id, updateDto);

            if (result.IsSuccess)
            {
                IsEditing = false;
                await Application.Current.MainPage.DisplayAlert("Succesvol", "Je profiel is bijgewerkt!", "OK");
            }
            else
            {
                
                await Application.Current.MainPage.DisplayAlert("Fout", $"Update kon niet worden uitgevoerd:\n{result.ErrorMessage}", "OK");
            }
        }

        [RelayCommand]
        void CancelEdit()
        {
            
            IsEditing = false;
        }


        [RelayCommand]
        async Task PickProfilePicture()
        {
            try
            {
                // 1. Foto kiezen uit de galerij
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Kies een profielfoto"
                });

                if (result != null)
                {
                    string? uploadedFileName = await _authService.UploadImageAsync(result);

                    if (!string.IsNullOrEmpty(uploadedFileName))
                    {
                        
                        CurrentUser.ProfileImage = uploadedFileName;

                        
                        OnPropertyChanged(nameof(ProfileImageUrl));

                       
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Foto selecteren fout: {ex.Message}");
            }
        }
    }
}
