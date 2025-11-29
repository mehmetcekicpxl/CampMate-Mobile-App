using CampMate.Models;
using CampMate.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CampMate.Views;

public partial class AddCampPage : ContentPage
{
    private string _selectedImageFileName;

    public AddCampPage()
    {
        InitializeComponent();
    }

    // Foto secme
    private async void PickImageButton_Clicked(object sender, EventArgs e)
    {
        _selectedImageFileName = await PickAndSaveImage();

        if (!string.IsNullOrEmpty(_selectedImageFileName))
        {
            // Foto secilmisse, AppDataDirectory’den göster
            var fullPath = Path.Combine(FileSystem.AppDataDirectory, "images", _selectedImageFileName);
            SelectedImage.Source = ImageSource.FromFile(fullPath);
        }
        else
        {
            // Foto secilmemisse, default bir resim göster
            SelectedImage.Source = "dotnet_bot.png"; // Resources/Images icinde olmalı
        }
    }

    // Cross-platform foto secme ve kaydetme
    private async Task<string> PickAndSaveImage()
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a camp image",
                FileTypes = FilePickerFileType.Images
            });

            if (result == null)
                return null;

            var newFileName = result.FileName;
            var imagesPath = Path.Combine(FileSystem.AppDataDirectory, "images");

            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);

            var newFilePath = Path.Combine(imagesPath, newFileName);

            using var sourceStream = await result.OpenReadAsync();
            using var newStream = File.OpenWrite(newFilePath);
            await sourceStream.CopyToAsync(newStream);

            return newFileName;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
            return null;
        }
    }

    
    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        var newCamp = new CampSite
        {
            Name = NameEntry.Text,
            Location = LocationEntry.Text,
            Facilities = GetSelectedFacilities(),
            Image = string.IsNullOrEmpty(_selectedImageFileName) ? "dotnet_bot.png" : _selectedImageFileName,
            Price = int.TryParse(PriceEntry.Text, out var price) ? price : 0,
            Stars = StarsPicker.SelectedItem != null ? int.Parse(StarsPicker.SelectedItem.ToString()) : 0
        };

        

        

        
        WeakReferenceMessenger.Default.Send(new NewCampMessage(newCamp));

        await DisplayAlert("Succes", "Camping succesvol toegevoegd!", "OK");

        
        NameEntry.Text = LocationEntry.Text  = PriceEntry.Text = "";
        _selectedImageFileName = null;
        SelectedImage.Source = null;
        StarsPicker.SelectedItem = null;

        // ga naar de vorige pagina(HomePage)
        await Shell.Current.GoToAsync("..");
    }

    private string GetSelectedFacilities()
    {
        var selected = new List<string>();

        if (ZwembadCheckBox.IsChecked)
            selected.Add("zwembad");
        if (FietsverhuurCheckBox.IsChecked)
            selected.Add("fietsverhuur");
        if (ElektriciteitCheckBox.IsChecked)
            selected.Add("Elektrisch");
        if (CafeCheckBox.IsChecked)
            selected.Add("cafe");

        
        return string.Join(",", selected);
    }



}
