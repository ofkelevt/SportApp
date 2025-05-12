using SportApp.ViewModels;
namespace SportApp.Views;

public partial class ViewEvent : ContentPage
{
    public ViewEvent(ViewEventViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ViewEventViewModel vm)
        {
            if (vm.RefreshCommand != null)
                vm.RefreshCommand.Execute(null);
        }
    }
}
