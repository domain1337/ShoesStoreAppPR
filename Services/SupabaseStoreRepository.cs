using ShoesStoreApp.Models;
using System.IO;

namespace ShoesStoreApp.Services;

public sealed class SupabaseStoreRepository : IStoreRepository
{
    public async Task<IReadOnlyList<Product>> GetProductsAsync() =>
        (await SupabaseService.Client.From<Product>().Get()).Models;

    public async Task<IReadOnlyList<Order>> GetOrdersAsync() =>
        (await SupabaseService.Client.From<Order>().Get()).Models;

    public async Task<IReadOnlyList<PickupPoint>> GetPickupPointsAsync() =>
        (await SupabaseService.Client.From<PickupPoint>().Get()).Models;

    public async Task<string> GetRoleAsync(Guid userId)
    {
        var result = await SupabaseService.Client.From<UserProfile>().Where(p => p.Id == userId).Get();
        return result.Models.FirstOrDefault()?.Role ?? "client";
    }

    public Task SaveProductAsync(Product product) => SupabaseService.Client.From<Product>().Upsert(product);

    public Task DeleteProductAsync(Guid productId) =>
        SupabaseService.Client.From<Product>().Where(p => p.Id == productId).Delete();

    public Task CreateOrderAsync(Order order) => SupabaseService.Client.From<Order>().Insert(order);

    public async Task<string> UploadProductImageAsync(string fileName, byte[] bytes)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (extension is not (".png" or ".jpg" or ".jpeg"))
            throw new ArgumentException("Выберите изображение PNG или JPEG.", nameof(fileName));
        var path = $"products/{Guid.NewGuid():N}{extension}";
        await SupabaseService.Client.Storage.From("images").Upload(bytes, path);
        return path;
    }
}
