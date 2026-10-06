using Supabase;

namespace ShoesStoreApp.Services;

public static class SupabaseService
{
    private static readonly Lazy<Client> LazyClient = new(() =>
    {
        var url = Environment.GetEnvironmentVariable("SHOES_SUPABASE_URL");
        var key = Environment.GetEnvironmentVariable("SHOES_SUPABASE_ANON_KEY");
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps ||
            string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Задайте SHOES_SUPABASE_URL и SHOES_SUPABASE_ANON_KEY (HTTPS URL и anon key).");

        return new Client(uri.ToString().TrimEnd('/'), key, new SupabaseOptions { AutoRefreshToken = true });
    });

    public static Client Client => LazyClient.Value;

    public static string PublicImageUrl(string path)
    {
        var url = Environment.GetEnvironmentVariable("SHOES_SUPABASE_URL")?.TrimEnd('/');
        return $"{url}/storage/v1/object/public/images/{Uri.EscapeDataString(path).Replace("%2F", "/")}";
    }
}
