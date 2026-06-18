using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using Newtonsoft.Json;
using Microsoft.Maui.Storage;

namespace catv1.Services;

public class MauiSessionHandler : IGotrueSessionPersistence<Session>
{
    private const string SessionKey = "supabase_session";

    public void SaveSession(Session session)
    {
        try
        {
            var json = JsonConvert.SerializeObject(session);
            SecureStorage.Default.SetAsync(SessionKey, json).Wait();
            System.Diagnostics.Debug.WriteLine("[MauiSessionHandler] Session successfully saved.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MauiSessionHandler] SaveSession error: {ex}");
        }
    }

    public void DestroySession()
    {
        try
        {
            SecureStorage.Default.Remove(SessionKey);
            System.Diagnostics.Debug.WriteLine("[MauiSessionHandler] Session successfully destroyed.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MauiSessionHandler] DestroySession error: {ex}");
        }
    }

    public Session? LoadSession()
    {
        try
        {
            var json = SecureStorage.Default.GetAsync(SessionKey).Result;
            if (string.IsNullOrEmpty(json))
            {
                System.Diagnostics.Debug.WriteLine("[MauiSessionHandler] No session found in SecureStorage.");
                return null;
            }
            
            System.Diagnostics.Debug.WriteLine("[MauiSessionHandler] Session found and loaded from SecureStorage.");
            return JsonConvert.DeserializeObject<Session>(json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MauiSessionHandler] LoadSession error: {ex}");
            return null;
        }
    }
}
