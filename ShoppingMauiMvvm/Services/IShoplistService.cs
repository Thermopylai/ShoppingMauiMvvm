using ShoppingMaui.Models;

namespace ShoppingMaui.Services;

public interface IShoplistService
{
    Task<IReadOnlyList<Shoplist>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Shoplist item, CancellationToken cancellationToken = default);

    Task CollectAsync(int itemId, CancellationToken cancellationToken = default);
}
