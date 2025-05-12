using SportApp.ViewModels;
namespace SportApp.Views;

public partial class UserDetailsPage : ContentPage
{
	public UserDetailsPage(UserDetailsViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is UserDetailsViewModel vm)
        {
            if (vm.RefreshCommand != null)
                vm.RefreshCommand.Execute(null);
        }
    }
}