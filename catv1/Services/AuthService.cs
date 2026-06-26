using Supabase;
using Supabase.Gotrue;

namespace catv1.Services;

public class AuthService : IAuthService
{
    private readonly Supabase.Client _supabase;

    public AuthService(Supabase.Client supabase)
    {
        _supabase = supabase;
    }

    public User? CurrentUser => _supabase.Auth.CurrentUser;

    public async Task<Session?> SignInAsync(string email, string password)
    {
        return await _supabase.Auth.SignIn(email, password);
    }

    public async Task<Session?> SignUpAsync(string email, string password, Dictionary<string, object> userData)
    {
        var options = new SignUpOptions { Data = userData };
        return await _supabase.Auth.SignUp(email, password, options);
    }

    public async Task SignOutAsync()
    {
        await _supabase.Auth.SignOut();
    }

    public async Task ResetPasswordAsync(string email)
    {
        await _supabase.Auth.ResetPasswordForEmail(email);
    }

    public async Task InitializeAsync()
    {
        await _supabase.InitializeAsync();
    }

    public Session? CurrentSession => _supabase.Auth.CurrentSession;
}
