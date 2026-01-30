namespace catv1;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        System.Diagnostics.Debug.WriteLine("CAT_LOG: App Constructor Start");
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine("CAT_LOG: App Constructor End");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: CreateWindow");
        var shell = _serviceProvider.GetRequiredService<AppShell>();
        return new Window(shell);
    }
}