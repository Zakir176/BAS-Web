namespace catv1;

public partial class App : Application
{
    //private readonly IServiceProvider _serviceProvider;
    //private readonly Supabase.Client _supabaseClient;

    public App(IServiceProvider serviceProvider)
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine($"CRITICAL_APP_ERROR (Unhandled): {e.ExceptionObject}");
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine($"CRITICAL_APP_ERROR (UnobservedTask): {e.Exception}");
            e.SetObserved();
        };

        System.Diagnostics.Debug.WriteLine("CAT_LOG: App Constructor Start");

        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CRITICAL_APP_ERROR (App Component Initialization): {ex}");
            throw; 
        }

        System.Diagnostics.Debug.WriteLine("CAT_LOG: App Constructor End");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: CreateWindow Start");
        try
        {
            var shell = Handler.MauiContext.Services.GetRequiredService<AppShell>();
            return new Window(shell);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CRITICAL_APP_ERROR (CreateWindow): {ex}");
            throw;
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        System.Diagnostics.Debug.WriteLine("CAT_LOG: App OnStart");
    }

    /*protected override Window CreateWindow(IActivationState? activationState)
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: CreateWindow");
        var shell = _serviceProvider.GetRequiredService<AppShell>();
        return new Window(shell);
    }*/
}