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

        builder.Services.AddSingleton(provider => new Supabase.Client(SupabaseConfig.Url, SupabaseConfig.Key));

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<StudentHomeViewModel>();
        // Add others if needed...

        // Pages
        builder.Services.AddTransient<Views.LoginPage>();
        builder.Services.AddTransient<Views.StudentHomePage>();
        // Add others if needed...

#if DEBUG
        builder.Logging.AddDebug();
#endif

        System.Diagnostics.Debug.WriteLine("CAT_LOG: CreateMauiApp End");
        return builder.Build();
    }
}
