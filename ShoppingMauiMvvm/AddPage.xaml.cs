using ShoppingMaui.ViewModels;

namespace ShoppingMaui;

public partial class AddPage : ContentPage
{
	public AddPage(AddItemViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
