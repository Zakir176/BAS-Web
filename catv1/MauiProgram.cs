using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        System.Diagnostics.Debug.WriteLine("CAT_LOG: CreateMauiApp End");
        return builder.Build();
    }
}
