
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