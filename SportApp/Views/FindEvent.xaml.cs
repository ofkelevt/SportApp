using SportApp.ViewModels;
using SportApp.Models;
namespace SportApp.Views;

public partial class FindEvent : ContentPage
{
	public FindEvent(FindEventsViewModel findEventsViewModel)
	{
        BindingContext = findEventsViewModel;
        InitializeComponent();
    }
}