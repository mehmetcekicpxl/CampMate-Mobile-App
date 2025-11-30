using CampMate.Models;
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
        [ObservableProperty]
        private User _currentUser;

        [ObservableProperty]
        private bool _isEditing;

        public ProfileViewModel()
        {
            // als default een gebruiker instellen (dit zou normaal van een service komen maar nu om alleen te testen )
            CurrentUser = new User();
            IsEditing = false; 
        }

        [RelayCommand]
        void EditProfile()
        {
            IsEditing = true;
        }

        [RelayCommand]
        async Task SaveChanges()
        {
           
            Console.WriteLine($"Gebruiker upgedated: {CurrentUser.Name}, {CurrentUser.Email}, {CurrentUser.Location},{CurrentUser.PhoneNumber}");

            IsEditing = false;

            
            await Application.Current.MainPage.DisplayAlert("succes", "Profile gegevens succes upgedeted !", "OK");
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
