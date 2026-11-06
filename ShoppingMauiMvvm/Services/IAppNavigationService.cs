namespace ShoppingMaui.Services;

public interface IAppNavigationService
{
    Task NavigateToAddItemAsync();

    Task GoBackAsync();
}
