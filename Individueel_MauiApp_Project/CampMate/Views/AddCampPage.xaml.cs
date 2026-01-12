//using CampMate.Shared.DTOs;
//using CampMate.Messages;
//using CampMate.Models;
//using CampMate.Services; 
//using CommunityToolkit.Mvvm.Messaging;
//using Microsoft.Maui.Storage; 

//namespace CampMate.Views;

//public partial class AddCampPage : ContentPage
//{

//    private readonly CampService _campService;
//    private readonly AuthService _authService;


//    private FileResult _selectedFileResult;


//    public AddCampPage(CampService campService, AuthService authService)
//    {
//        InitializeComponent();
//        _campService = campService;
//        _authService = authService;
//    }

//    private async void PickImageButton_Clicked(object sender, EventArgs e)
//    {
//        try
//        {
//            // Selecteer een afbeelding uit de galerij
//            var result = await FilePicker.Default.PickAsync(new PickOptions
//            {
//                PickerTitle = "Select a camp image",
//                FileTypes = FilePickerFileType.Images
//            });

//            if (result != null)
//            {

//                _selectedFileResult = result;

//                // 2. Tijdelijk op het scherm weergeven om de gebruiker een voorbeeld te tonen
//                var stream = await result.OpenReadAsync();
//                SelectedImage.Source = ImageSource.FromStream(() => stream);
//            }
//        }
//        catch (Exception ex)
//        {
//            await DisplayAlert("Fout", "Afbeelding kan niet worden geselecteerd: " + ex.Message, "OK");
//        }
//    }

//    private async void SaveButton_Clicked(object sender, EventArgs e)
//    {

//        if (string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(LocationEntry.Text))
//        {
//            await DisplayAlert("Waarschuwing", "Vul de velden voor naam en locatie in.", "OK");
//            return;
//        }


//    ((Button)sender).IsEnabled = false;

//        string serverImageName = "dotnet_bot.png"; 

//        try
//        {
//            // 2. FOTO'S UPLOADEN
//            if (_selectedFileResult != null)
//            {
//                var uploadedName = await _authService.UploadImageAsync(_selectedFileResult);
//                if (!string.IsNullOrEmpty(uploadedName))
//                {
//                    serverImageName = uploadedName; 
//                }
//            }


//            int currentUserId = Preferences.Get("UserId", 0);

//            var dto = new CampSiteDto
//            {
//                Name = NameEntry.Text,
//                Location = LocationEntry.Text,
//                Facilities = GetSelectedFacilities(),
//                Price = int.TryParse(PriceEntry.Text, out var price) ? price : 0,
//                Image = serverImageName,
//                UserId = currentUserId
//            };


//            CampSiteDto createdCamp = await _campService.AddCampAsync(dto);

//            if (createdCamp != null) 
//            {

//                if (!string.IsNullOrEmpty(createdCamp.Image) && !createdCamp.Image.StartsWith("http"))
//                {
//                    createdCamp.Image = $"http://10.0.2.2:5225/images/{createdCamp.Image}";
//                }


//                WeakReferenceMessenger.Default.Send(new NewCampMessage(createdCamp));

//                await DisplayAlert("Succesvol", "De camping is succesvol toegevoegd!", "OK");


//                NameEntry.Text = LocationEntry.Text = PriceEntry.Text = "";
//                SelectedImage.Source = null;
//                _selectedFileResult = null;


//                await Shell.Current.GoToAsync("..");
//            }
//            else
//            {
//                await DisplayAlert("Fout", "Er is een probleem opgetreden bij het toevoegen van het kamp. Probeer het opnieuw.", "OK");
//            }
//        }
//        catch (Exception ex)
//        {
//            await DisplayAlert("Fout", $"Onverwachte fout: {ex.Message}", "OK");
//        }
//        finally
//        {

//            ((Button)sender).IsEnabled = true;
//        }
//    }

//    private string GetSelectedFacilities()
//    {
//        var selected = new List<string>();

//        if (ZwembadCheckBox.IsChecked) selected.Add("zwembad");
//        if (FietsverhuurCheckBox.IsChecked) selected.Add("fietsverhuur");
//        if (ElektriciteitCheckBox.IsChecked) selected.Add("Elektrisch");
//        if (CafeCheckBox.IsChecked) selected.Add("cafe");

//        return string.Join(",", selected);
//    }
//}

using CampMate.ViewModels;

namespace CampMate.Views;

public partial class AddCampPage : ContentPage
{
    public AddCampPage(AddCampViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}