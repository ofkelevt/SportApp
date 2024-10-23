using SportApp.ViewModels;
namespace SportApp.Views;

public partial class CrateEvent : ContentPage
{
	public CrateEvent(CrateEventViewModel vm)
	{
        BindingContext = vm;
        InitializeComponent();
	}
}