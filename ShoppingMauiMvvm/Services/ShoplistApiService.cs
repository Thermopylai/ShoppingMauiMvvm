using System.Net.Http.Json;
using ShoppingMaui.Models;

namespace ShoppingMaui.Services;

public sealed class ShoplistApiService(HttpClient httpClient) : IShoplistService
{
    public async Task<IReadOnlyList<Shoplist>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/shoplist", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Shoplist>>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("The shopping list response was empty.");
    }

    public async Task AddAsync(Shoplist item, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/shoplist", item, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task CollectAsync(int itemId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/shoplist/{itemId}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
