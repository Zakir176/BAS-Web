namespace catv1;

public partial class App : Application
{
    //private readonly IServiceProvider _serviceProvider;
    //private readonly Supabase.Client _supabaseClient;

    public App(IServiceProvider serviceProvider)//, Supabase.Client supabaseClient)
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

        //_serviceProvider = serviceProvider;
        //_supabaseClient = supabaseClient;
        System.Diagnostics.Debug.WriteLine("CAT_LOG: App Constructor Start");

        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CRITICAL_APP_ERROR (InitializeComponent): {ex}");
            // We can't easily show a dialog here if the app is crashing, but we've logged it.
            throw; // Re-throw to allow normal crash behavior but with logging
        }

        System.Diagnostics.Debug.WriteLine("CAT_LOG: App Constructor End");
    }

    protected override void OnStart()
    {
        base.OnStart();
        try
        {
            System.Diagnostics.Debug.WriteLine("CAT_LOG: Initializing Supabase...");
            //await _supabaseClient.InitializeAsync();
            System.Diagnostics.Debug.WriteLine("CAT_LOG: Supabase Initialized.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CAT_LOG_ERROR: Supabase Initialization Failed: {ex}");
        }
    }

    /*protected override Window CreateWindow(IActivationState? activationState)
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: CreateWindow");
        var shell = _serviceProvider.GetRequiredService<AppShell>();
        return new Window(shell);
    }*/
}