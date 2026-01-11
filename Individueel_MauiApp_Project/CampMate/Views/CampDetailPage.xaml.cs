using CampMate.Shared.DTOs;
using CampMate.ViewModels; 

namespace CampMate.Views;

public partial class CampDetailPage : ContentPage
{
    
    public CampDetailPage(CampDetailViewModel viewModel)
    {
        InitializeComponent();

       
        BindingContext = viewModel;
    }
}