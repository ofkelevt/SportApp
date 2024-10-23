using SportApp.ViewModels;
namespace SportApp.Views;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
        this.BindingContext = new SignUpViewModel();
    }
}