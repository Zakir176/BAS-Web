namespace catv1;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        System.Diagnostics.Debug.WriteLine("CAT_LOG: App Constructor Start");
        InitializeComponent();

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine($"CRITICAL_APP_ERROR (Unhandled): {e.ExceptionObject}");
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine($"CRITICAL_APP_ERROR (UnobservedTask): {e.Exception}");
            e.SetObserved();
        };

        System.Diagnostics.Debug.WriteLine("CAT_LOG: App Constructor End");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: CreateWindow");
        var shell = _serviceProvider.GetRequiredService<AppShell>();
        return new Window(shell);
    }
}