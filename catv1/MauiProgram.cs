using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;
using catv1.ViewModels;

namespace catv1;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: CreateMauiApp Start");
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        System.Diagnostics.Debug.WriteLine("CAT_LOG: Registering Services");

        builder.Services.AddSingleton(provider =>
        {
            System.Diagnostics.Debug.WriteLine("CAT_LOG: Creating Supabase Client");
            return new Supabase.Client(SupabaseConfig.Url, SupabaseConfig.Key, new Supabase.SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            });
        });

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<StudentHomeViewModel>();
        builder.Services.AddTransient<HistoryViewModel>();
        builder.Services.AddTransient<ScanViewModel>();

        // Pages
        builder.Services.AddTransient<Views.LoginPage>();
        builder.Services.AddTransient<Views.StudentHomePage>();
        builder.Services.AddTransient<Views.HistoryPage>();
        builder.Services.AddTransient<Views.ScanPage>();
        builder.Services.AddTransient<Views.LecturerHomePage>();
        builder.Services.AddTransient<Views.SignUpPage>();

        // Shell
        builder.Services.AddSingleton<AppShell>();

        System.Diagnostics.Debug.WriteLine("CAT_LOG: Services Registered");

#if DEBUG
        builder.Logging.AddDebug();
#endif

        System.Diagnostics.Debug.WriteLine("CAT_LOG: CreateMauiApp End");
        return builder.Build();
    }
}
