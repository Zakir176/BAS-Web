namespace catv1;

/// <summary>
/// Template for local secrets. Copy this file to Secrets.cs (which is gitignored)
/// and fill in the real values from the Supabase dashboard → Settings → API.
///
/// NEVER put actual keys in this file.
/// </summary>
internal static class SecretsTemplate
{
    internal const string SupabaseUrl = "https://YOUR_PROJECT_REF.supabase.co";
    internal const string SupabaseAnonKey = "YOUR_SUPABASE_ANON_KEY";
}
