namespace catv1;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

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

        _serviceProvider = serviceProvider;
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

    protected override Window CreateWindow(IActivationState? activationState)
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: CreateWindow");
        var shell = _serviceProvider.GetRequiredService<AppShell>();
        return new Window(shell);
    }
}