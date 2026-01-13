
using CampMate.ViewModels;

namespace CampMate.Views;

public partial class HomePage : ContentPage
{
    private readonly HomePageViewModel _viewModel;
    // Injecteer ViewModel
    public HomePage(HomePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

       
        if (_viewModel != null)
        {

            //Als het filter niet is toegepast, laad dan alle gegevens.

            if (!_viewModel.IsFilterApplied)
            {
                if (_viewModel.LoadCampsCommand.CanExecute(null))
                {
                    _viewModel.LoadCampsCommand.Execute(null);
                }
            }
            
        }
    }
}