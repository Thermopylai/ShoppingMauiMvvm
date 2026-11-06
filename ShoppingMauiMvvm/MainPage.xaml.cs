using ShoppingMaui.ViewModels;

namespace ShoppingMaui;

public partial class MainPage : ContentPage
{
    private readonly ShoplistViewModel viewModel;

    public MainPage(ShoplistViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (viewModel.LoadCommand.CanExecute(null))
        {
            viewModel.LoadCommand.Execute(null);
        }
    }
}
