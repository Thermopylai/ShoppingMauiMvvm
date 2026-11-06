using Microsoft.Extensions.Logging;
using ShoppingMaui.Services;
using ShoppingMaui.ViewModels;

namespace ShoppingMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri("https://shoppingbackenddoomguy-a2fqb6c4ecd0h7at.swedencentral-01.azurewebsites.net/")
        });
        builder.Services.AddSingleton<IShoplistService, ShoplistApiService>();
        builder.Services.AddSingleton<IAppNavigationService, ShellNavigationService>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AddPage>();
        builder.Services.AddTransient<ShoplistViewModel>();
        builder.Services.AddTransient<AddItemViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
