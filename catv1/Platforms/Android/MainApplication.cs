using Android.App;
using Android.Runtime;

namespace catv1;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp()
    {
        try
        {
            return MauiProgram.CreateMauiApp();
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("CAT_CRASH_ERROR", $"Fatal exception during CreateMauiApp: {ex}");
            throw;
        }
    }
}
