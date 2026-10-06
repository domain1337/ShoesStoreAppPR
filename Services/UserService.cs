namespace ShoesStoreApp.Services;

public static class UserService
{
    public static string CurrentRole { get; private set; } = "guest";
    public static string? UserEmail { get; private set; }

    public static bool IsAdmin => CurrentRole == "admin";
    public static bool IsManagerOrAdmin => IsAdmin || CurrentRole == "manager";
    public static bool IsAuthenticated => CurrentRole != "guest";

    public static void SetAuthenticated(string email, string role)
    {
        UserEmail = email;
        CurrentRole = role is "admin" or "manager" ? role : "client";
    }

    public static void Reset()
    {
        CurrentRole = "guest";
        UserEmail = null;
    }
}
