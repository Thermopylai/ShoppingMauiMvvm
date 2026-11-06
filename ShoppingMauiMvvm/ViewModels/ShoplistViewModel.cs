using System.Collections.ObjectModel;
using System.Windows.Input;
using ShoppingMaui.Models;
using ShoppingMaui.Services;

namespace ShoppingMaui.ViewModels;

public sealed partial class ShoplistViewModel : BindableObject
{
    private readonly IShoplistService shoplistService;
    private readonly IAppNavigationService navigationService;
    private ObservableCollection<Shoplist> shoplist = [];
    private Shoplist? selectedItem;
    private string errorMessage = string.Empty;
    private bool isLoading;
    private bool isNavigating;

    public ShoplistViewModel(IShoplistService shoplistService, IAppNavigationService navigationService)
    {
        this.shoplistService = shoplistService;
        this.navigationService = navigationService;
        LoadCommand = new Command(async () => await LoadAsync(), () => !IsLoading);
        OpenAddItemCommand = new Command(async () => await OpenAddItemAsync(), () => !IsLoading && !isNavigating);
        CollectSelectedItemCommand = new Command(async () => await CollectSelectedItemAsync(), () => CanCollectSelectedItem);
    }

    public ObservableCollection<Shoplist> Shoplist
    {
        get => shoplist;
        private set
        {
            shoplist = value;
            OnPropertyChanged();
        }
    }

    public Shoplist? SelectedItem
    {
        get => selectedItem;
        set
        {
            if (selectedItem == value)
            {
                return;
            }

            selectedItem = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanCollectSelectedItem));
            ((Command)CollectSelectedItemCommand).ChangeCanExecute();
        }
    }

    public string ErrorMessage
    {
        get => errorMessage;
        private set
        {
            if (errorMessage == value)
            {
                return;
            }

            errorMessage = value;
            OnPropertyChanged();
        }
    }

    public bool IsLoading
    {
        get => isLoading;
        private set
        {
            if (isLoading == value)
            {
                return;
            }

            isLoading = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanCollectSelectedItem));
            ((Command)LoadCommand).ChangeCanExecute();
            ((Command)OpenAddItemCommand).ChangeCanExecute();
            ((Command)CollectSelectedItemCommand).ChangeCanExecute();
        }
    }

    public bool CanCollectSelectedItem => SelectedItem is not null && !IsLoading;

    public ICommand LoadCommand { get; }

    public ICommand OpenAddItemCommand { get; }

    public ICommand CollectSelectedItemCommand { get; }

    private async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var items = await shoplistService.GetAllAsync();
            SelectedItem = null;
            Shoplist = new ObservableCollection<Shoplist>(items);
        }
        catch (Exception exception)
        {
            ErrorMessage = $"Unable to load the shopping list: {exception.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task OpenAddItemAsync()
    {
        isNavigating = true;
        ((Command)OpenAddItemCommand).ChangeCanExecute();
        ErrorMessage = string.Empty;

        try
        {
            await navigationService.NavigateToAddItemAsync();
        }
        catch (Exception exception)
        {
            ErrorMessage = $"Unable to open the add-item page: {exception.Message}";
        }
        finally
        {
            isNavigating = false;
            ((Command)OpenAddItemCommand).ChangeCanExecute();
        }
    }

    private async Task CollectSelectedItemAsync()
    {
        if (SelectedItem is not { } item)
        {
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            await shoplistService.CollectAsync(item.Id);
            SelectedItem = null;
            Shoplist.Remove(item);

            try
            {
                Shoplist = new ObservableCollection<Shoplist>(await shoplistService.GetAllAsync());
            }
            catch (Exception exception)
            {
                ErrorMessage = $"The item was collected, but the shopping list could not be refreshed: {exception.Message}";
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = $"Unable to collect the item: {exception.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
