using Microsoft.Maui.Controls;

public class NavigationService
{
    private readonly INavigation _navigation;

    public NavigationService(INavigation navigation)
    {
        _navigation = navigation;
    }

    public async Task NavigateToAsync(Page page)
    {
        await _navigation.PushAsync(page);
    }
}
