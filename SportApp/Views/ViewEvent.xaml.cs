using SportApp.ViewModels;
namespace SportApp.Views;

public partial class ViewEvent : ContentPage
{
    public ViewEvent(ViewEventViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
