using SportApp.ViewModels;
using SportApp.Services;

namespace SportApp.Views;

public partial class LoginView : ContentPage
{
	public LoginView(LoginViewModel vm)
	{
		this.BindingContext = vm;
		InitializeComponent();
		
	}
}