using SportApp.ViewModels;
namespace SportApp.Views;

public partial class SignUpPage : ContentPage
{
	public SignUpPage(SignUpViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}