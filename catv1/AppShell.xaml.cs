namespace catv1;

public partial class AppShell : Shell
{
    public AppShell()
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: AppShell Constructor Start");
        InitializeComponent();
        Routing.RegisterRoute("signup", typeof(Views.SignUpPage));
        System.Diagnostics.Debug.WriteLine("CAT_LOG: AppShell Constructor End");
    }
}
