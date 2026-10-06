using ShoesStoreApp.Models;

namespace ShoesStoreApp.Services;

public interface IStoreRepository
{
    Task<IReadOnlyList<Product>> GetProductsAsync();
    Task<IReadOnlyList<Order>> GetOrdersAsync();
    Task<IReadOnlyList<PickupPoint>> GetPickupPointsAsync();
    Task<string> GetRoleAsync(Guid userId);
    Task SaveProductAsync(Product product);
    Task DeleteProductAsync(Guid productId);
    Task CreateOrderAsync(Order order);
    Task<string> UploadProductImageAsync(string fileName, byte[] bytes);
}
