namespace ShoesStoreApp.Services;

public static class StoreRepository
{
    public static IStoreRepository Current { get; set; } = new SupabaseStoreRepository();
}
