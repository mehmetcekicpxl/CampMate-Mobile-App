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
        private readonly AuthService _authService;

        [ObservableProperty]
        private User _currentUser;

        [ObservableProperty]
        private bool _isEditing;

        public ProfileViewModel(AuthService authService)
        {
            // als default een gebruiker instellen (dit zou normaal van een service komen maar nu om alleen te testen )
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
                    Location = userDto.Location
                };
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

            // 1. Maak het DTO-object klaar met de nieuwe gegevens van het scherm
            var updateDto = new UserUpdateDto
            {
                Name = CurrentUser.Name,
                Email = CurrentUser.Email,
                PhoneNumber = CurrentUser.PhoneNumber,
                Location = CurrentUser.Location
            };

            // 2. Roep de service aan om de update naar de API te sturen
            bool success = await _authService.UpdateUserProfileAsync(CurrentUser.Id, updateDto);

            if (success)
            {
                IsEditing = false; 
                await Application.Current.MainPage.DisplayAlert("Succes", "Je profiel is succesvol bijgewerkt!", "OK");

                
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Update mislukt. Controleer je gegevens (bijv. e-mail).", "OK");

                
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
            // om een profielfoto te kiezen
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Kies een profielfoto"
            });

            if (result != null)
            {
               
                Console.WriteLine($"De path: {result.FullPath}");
            }
        }
    }
}
