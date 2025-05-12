using SportApp.ViewModels;
namespace SportApp.Views;

public partial class AdminPage : ContentPage
{
	public AdminPage(AdminViewModel vm)
	{
		BindingContext = vm;
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AdminViewModel vm)
        {
            if (vm.RefreshCommand != null)
                vm.RefreshCommand.Execute(null);
        }
    }
}