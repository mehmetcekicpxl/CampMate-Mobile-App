using CampMate.Messages;
using CampMate.Services;
using CampMate.Shared.DTOs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;


namespace CampMate.ViewModels
{
    public partial class AddCampViewModel : ObservableObject
    {
        private readonly CampService _campService;
        private readonly AuthService _authService;

        private FileResult _selectedFileResult;

        // Formuliervelden
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _location;

        [ObservableProperty]
        private string _priceText; // Entry text is string, we parsen later naar int

        // Faciliteiten Checkboxes
        [ObservableProperty]
        private bool _hasZwembad;

        [ObservableProperty]
        private bool _hasFietsverhuur;

        [ObservableProperty]
        private bool _hasElektriciteit;

        [ObservableProperty]
        private bool _hasCafe;

        // Afbeelding voorbeeld
        [ObservableProperty]
        private ImageSource _selectedImageSource;

        [ObservableProperty]
        private bool _isBusy; // Om dubbele klikken te voorkomen


        [ObservableProperty]
        private string _websiteUrl;

        public AddCampViewModel(CampService campService, AuthService authService)
        {
            _campService = campService;
            _authService = authService;
        }

        [RelayCommand]
        async Task PickImageAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a camp image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    _selectedFileResult = result;
                    var stream = await result.OpenReadAsync();
                    SelectedImageSource = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Fout", "Afbeelding kan niet worden geselecteerd: " + ex.Message, "OK");
            }
        }

        [RelayCommand]
        async Task SaveCampAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Location))
            {
                await Shell.Current.DisplayAlert("Waarschuwing", "Vul de velden voor naam en locatie in.", "OK");
                return;
            }

            IsBusy = true;
            string serverImageName = "dotnet_bot.png";

            try
            {
                // 1. Upload Afbeelding
                if (_selectedFileResult != null)
                {
                    var uploadedName = await _authService.UploadImageAsync(_selectedFileResult);
                    if (!string.IsNullOrEmpty(uploadedName))
                    {
                        serverImageName = uploadedName;
                    }
                }

                // 2. DTO Aanmaken
                int currentUserId = Preferences.Get("UserId", 0);

                var dto = new CampSiteDto
                {
                    Name = Name,
                    Location = Location,
                    Facilities = GetSelectedFacilities(),
                    Price = int.TryParse(PriceText, out var price) ? price : 0,
                    Image = serverImageName,
                    UserId = currentUserId,
                    WebsiteUrl = WebsiteUrl
                };

                // 3. Opslaan via Service
                var createdCamp = await _campService.AddCampAsync(dto);

                if (createdCamp != null)
                {
                    // Image URL fix voor weergave
                    if (!string.IsNullOrEmpty(createdCamp.Image) && !createdCamp.Image.StartsWith("http"))
                    {
                        createdCamp.Image = $"http://10.0.2.2:5225/images/{createdCamp.Image}";
                    }

                    // HomePage op de hoogte stellen
                    WeakReferenceMessenger.Default.Send(new NewCampMessage(createdCamp));

                    await Shell.Current.DisplayAlert("Succesvol", "De camping is succesvol toegevoegd!", "OK");

                    // Velden resetten
                    ClearForm();

                    // Terug naar vorige pagina
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Fout", "Er is een probleem opgetreden bij het toevoegen.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Fout", $"Onverwachte fout: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private string GetSelectedFacilities()
        {
            var selected = new List<string>();
            if (HasZwembad) selected.Add("zwembad");
            if (HasFietsverhuur) selected.Add("fietsverhuur");
            if (HasElektriciteit) selected.Add("Elektrisch");
            if (HasCafe) selected.Add("cafe");
            return string.Join(",", selected);
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Location = string.Empty;
            PriceText = string.Empty;
            SelectedImageSource = null;
            _selectedFileResult = null;
            HasZwembad = false;
            HasFietsverhuur = false;
            HasElektriciteit = false;
            HasCafe = false;
            WebsiteUrl = string.Empty;
        }
    }
}