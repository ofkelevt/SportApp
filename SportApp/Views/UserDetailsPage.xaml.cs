using SportApp.ViewModels;
namespace SportApp.Views;

public partial class UserDetailsPage : ContentPage
{
	public UserDetailsPage(UserDetailsViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}