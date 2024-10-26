using SportApp.Models;
using SportApp.ViewModels;
namespace SportApp.Views;

public partial class Logout : ContentPage
{
	public Logout(LogoutViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}