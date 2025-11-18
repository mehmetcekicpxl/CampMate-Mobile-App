using CampMate.Messages;
using CampMate.Models;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;

namespace CampMate.Views
{
    public partial class FilterPage : ContentPage
    {
        public FilterPage()
        {
            InitializeComponent();
        }

        private void ApplyFilters_Clicked(object sender, EventArgs e)
        {
            
            string? selectedLocation = LocationPicker.SelectedItem?.ToString();

            // alleen de gekozende var selectedFacilities = new List<string>();

            var selectedFacilities = new List<string>();

            if (ZwembadCheckBox.IsChecked) selectedFacilities.Add("zwembad");
            if (ElektriciteitCheckBox.IsChecked) selectedFacilities.Add("Elektrisch");
            if (FietsverhuurCheckBox.IsChecked) selectedFacilities.Add("fietsverhuur");
            if (CafeCheckBox.IsChecked) selectedFacilities.Add("cafe");
            


            
            var options = new FilterOptions
            {
                Location = selectedLocation,
                Facilities = selectedFacilities.Count > 0 ? selectedFacilities : null
            };

           
            WeakReferenceMessenger.Default.Send(new FilterCriteriaMessage(options));

            //  Dicht de filterpage
            Navigation.PopAsync();
        }
    }

  
    
}
