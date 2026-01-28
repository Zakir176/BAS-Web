namespace catv1;

public partial class App : Application
{
    public App()
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: App Constructor Start");
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine("CAT_LOG: App Constructor End");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: CreateWindow");
        return new Window(new AppShell());
    }
}