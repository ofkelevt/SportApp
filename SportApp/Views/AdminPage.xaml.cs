using SportApp.ViewModels;
namespace SportApp.Views;

public partial class AdminPage : ContentPage
{
	public AdminPage(AdminViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
}