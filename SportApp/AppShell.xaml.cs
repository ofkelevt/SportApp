using SportApp.Views;
using SportApp.Services;
using SportApp.Models;
using SportApp.ViewModels;
namespace SportApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		BindingContext = new AppShellViewModel();
		Routing.RegisterRoute(nameof(ViewEvent), typeof(ViewEvent));
		Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
		Routing.RegisterRoute(nameof(CrateEvent), typeof(CrateEvent));
		Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
		
	}
}
