using SportApp.ViewModels;
using SportApp.Models;
namespace SportApp.Views;

public partial class FindEvent : ContentPage
{
    public FindEvent(FindEventsViewModel findEventsViewModel)
    {
        InitializeComponent();
        BindingContext = findEventsViewModel;
    }
        protected override  void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is FindEventsViewModel vm)
        {
            if(vm.RefreshCommand != null)
                vm.RefreshCommand.Execute(null);
        }
    }
}
