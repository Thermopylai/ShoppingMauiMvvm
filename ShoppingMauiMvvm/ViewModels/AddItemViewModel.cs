using System.Windows.Input;
using ShoppingMaui.Models;
using ShoppingMaui.Services;

namespace ShoppingMaui.ViewModels;

public sealed class AddItemViewModel : BindableObject
{
    private readonly IShoplistService shoplistService;
    private readonly IAppNavigationService navigationService;
    private string newItemName = string.Empty;
    private string newItemQuantity = string.Empty;
    private string errorMessage = string.Empty;
    private bool isBusy;

    public AddItemViewModel(IShoplistService shoplistService, IAppNavigationService navigationService)
    {
        this.shoplistService = shoplistService;
        this.navigationService = navigationService;
        AddItemCommand = new Command(async () => await AddItemAsync(), () => !IsBusy);
    }

    public string NewItemName
    {
        get => newItemName;
        set
        {
            if (newItemName == value)
            {
                return;
            }

            newItemName = value;
            OnPropertyChanged();
        }
    }

    public string NewItemQuantity
    {
        get => newItemQuantity;
        set
        {
            if (newItemQuantity == value)
            {
                return;
            }

            newItemQuantity = value;
            OnPropertyChanged();
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

    public bool IsBusy
    {
        get => isBusy;
        private set
        {
            if (isBusy == value)
            {
                return;
            }

            isBusy = value;
            OnPropertyChanged();
            ((Command)AddItemCommand).ChangeCanExecute();
        }
    }

    public ICommand AddItemCommand { get; }

    private async Task AddItemAsync()
    {
        if (!int.TryParse(NewItemQuantity, out var quantity) || quantity <= 0)
        {
            ErrorMessage = "Enter a quantity greater than zero.";
            return;
        }

        if (string.IsNullOrWhiteSpace(NewItemName))
        {
            ErrorMessage = "Enter an item name.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            await shoplistService.AddAsync(new Shoplist
            {
                Item = NewItemName.Trim(),
                Amount = quantity
            });

            NewItemName = string.Empty;
            NewItemQuantity = string.Empty;

            try
            {
                await navigationService.GoBackAsync();
            }
            catch (Exception exception)
            {
                ErrorMessage = $"The item was added, but the shopping list could not be opened: {exception.Message}";
            }
        }
        catch (Exception exception)
        {
            ErrorMessage = $"Unable to add the item: {exception.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
