using Supabase.Gotrue;

namespace catv1.Services;

public interface IAuthService
{
    User? CurrentUser { get; }
    Task<Session?> SignInAsync(string email, string password);
    Task<Session?> SignUpAsync(string email, string password, Dictionary<string, object> userData);
    Task SignOutAsync();
    Task ResetPasswordAsync(string email);
    Task InitializeAsync();
    Session? CurrentSession { get; }
}
