namespace ShoesStoreApp.Services;

public static class AuthService
{
    public static async Task<(string Email, string Role)> SignInAsync(string email, string password)
    {
        var session = await SupabaseService.Client.Auth.SignIn(email, password);
        if (session?.User?.Id is not string id || !Guid.TryParse(id, out var userId) ||
            string.IsNullOrWhiteSpace(session.User.Email))
            throw new InvalidOperationException("Сервер не вернул данные пользователя.");
        var role = await StoreRepository.Current.GetRoleAsync(userId);
        return (session.User.Email, role);
    }

    public static async Task SignUpAsync(string email, string password)
    {
        var session = await SupabaseService.Client.Auth.SignUp(email, password);
        if (session is null) throw new InvalidOperationException("Сервер не подтвердил регистрацию.");
    }

    public static Task SignOutAsync() => SupabaseService.Client.Auth.SignOut();
}
