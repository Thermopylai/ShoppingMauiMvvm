namespace ShoppingMaui.Services;

public sealed class ShellNavigationService : IAppNavigationService
{
    public Task NavigateToAddItemAsync()
    {
        return Shell.Current.GoToAsync(nameof(AddPage));
    }

    public Task GoBackAsync()
    {
        return Shell.Current.GoToAsync("//MainPage");
    }
}
