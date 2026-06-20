using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;
using Plugin.LocalNotification;
using Plugin.Fingerprint.Abstractions;
using Plugin.Fingerprint;
using catv1.ViewModels;
using catv1.Services;

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
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            });

        System.Diagnostics.Debug.WriteLine("CAT_LOG: Registering Services");

        builder.Services.AddSingleton<Supabase.Client>(provider =>
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("CAT_LOG: Creating Supabase Client");
                var client = new Supabase.Client(SupabaseConfig.Url, SupabaseConfig.Key, new Supabase.SupabaseOptions
                {
                    AutoRefreshToken = true,
                    AutoConnectRealtime = true,
                    SessionHandler = new catv1.Services.MauiSessionHandler()
                });
                return client;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CAT_LOG_ERROR: Supabase Client Creation Failed: {ex}");
                throw; // Rethrow to ensure we know why it failed during startup
            }
        });

        // Application Services
        builder.Services.AddSingleton<IOfflineSyncService, OfflineSyncService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<ICourseService, CourseService>();
        builder.Services.AddSingleton<IAttendanceService, AttendanceService>();
        builder.Services.AddSingleton<IProfileService, ProfileService>();
        builder.Services.AddSingleton<IFingerprint>(CrossFingerprint.Current);

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<StudentHomeViewModel>();
        builder.Services.AddTransient<BarcodeViewModel>();
        builder.Services.AddTransient<SignUpViewModel>();
        builder.Services.AddTransient<LecturerHomeViewModel>();
        builder.Services.AddTransient<LecturerReportsViewModel>();
        builder.Services.AddTransient<StudentHistoryViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<CourseListViewModel>();
        builder.Services.AddTransient<NotificationsViewModel>();
        builder.Services.AddTransient<ScanViewModel>();

        // Views
        builder.Services.AddTransient<Views.LoginPage>();
        builder.Services.AddTransient<Views.SignUpPage>();
        builder.Services.AddTransient<Views.StudentHomePage>();
        builder.Services.AddTransient<Views.LecturerHomePage>();
        builder.Services.AddTransient<Views.LecturerReportsPage>();
        builder.Services.AddTransient<Views.StudentHistoryPage>();
        builder.Services.AddTransient<Views.SettingsPage>();
        builder.Services.AddTransient<Views.CourseListPage>();
        builder.Services.AddTransient<Views.NotificationsPage>();
        builder.Services.AddTransient<Views.BarcodePage>();
        builder.Services.AddTransient<Views.ScanPage>();

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
