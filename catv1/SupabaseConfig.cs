namespace catv1;

/// <summary>
/// Supabase connection config. Actual values are loaded from Secrets.cs (gitignored).
/// See Secrets.template.cs for setup instructions.
/// </summary>
public static class SupabaseConfig
{
    public static string Url => Secrets.SupabaseUrl;
    public static string Key => Secrets.SupabaseAnonKey;
}
