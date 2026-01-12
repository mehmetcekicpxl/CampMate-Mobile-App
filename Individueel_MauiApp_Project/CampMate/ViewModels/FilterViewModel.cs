using CampMate.Messages;
using CampMate.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace CampMate.ViewModels
{
    public partial class FilterViewModel : ObservableObject
    {
        // Locaties voor de Picker
        public ObservableCollection<string> Locations { get; } = new();

        // Geselecteerde waarden
        [ObservableProperty]
        private string _selectedLocation;

        [ObservableProperty]
        private bool _hasZwembad;

        [ObservableProperty]
        private bool _hasFietsverhuur;

        [ObservableProperty]
        private bool _hasElektriciteit;

        [ObservableProperty]
        private bool _hasCafe;

        public FilterViewModel(List<CampSite> allCamps)
        {
            LoadLocations(allCamps);
        }

        private void LoadLocations(List<CampSite> camps)
        {
            if (camps == null) return;

            // Unieke locaties ophalen
            var uniqueLocations = camps.Select(c => c.Location).Distinct().ToList();

            Locations.Clear();
            foreach (var loc in uniqueLocations)
            {
                Locations.Add(loc);
            }
        }

        [RelayCommand]
        async Task ApplyFiltersAsync()
        {
            // Faciliteiten lijst samenstellen
            var selectedFacilities = new List<string>();

            if (HasZwembad) selectedFacilities.Add("zwembad");
            if (HasElektriciteit) selectedFacilities.Add("Elektrisch"); // Backend naamgeving checken
            if (HasFietsverhuur) selectedFacilities.Add("fietsverhuur");
            if (HasCafe) selectedFacilities.Add("cafe");

            // Filter opties object maken
            var options = new FilterOptions
            {
                Location = SelectedLocation,
                Facilities = selectedFacilities.Count > 0 ? selectedFacilities : null
            };

            // Stuur bericht naar HomePage
            WeakReferenceMessenger.Default.Send(new FilterCriteriaMessage(options));

            // Sluit de pagina
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}