namespace catv1;

public partial class AppShell : Shell
{
    public AppShell()
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: AppShell Constructor Start");
        InitializeComponent();
        Routing.RegisterRoute("signup", typeof(Views.SignUpPage));
        Routing.RegisterRoute("barcode", typeof(Views.BarcodePage));
        System.Diagnostics.Debug.WriteLine("CAT_LOG: AppShell Constructor End");
    }
}
