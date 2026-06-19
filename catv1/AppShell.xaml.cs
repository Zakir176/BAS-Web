namespace catv1;

public partial class AppShell : Shell
{
    public AppShell()
    {
        System.Diagnostics.Debug.WriteLine("CAT_LOG: AppShell Constructor Start");
        InitializeComponent();
        Routing.RegisterRoute("signup", typeof(Views.SignUpPage));
        Routing.RegisterRoute("barcode", typeof(Views.BarcodePage));
        Routing.RegisterRoute("settings", typeof(Views.SettingsPage));
        Routing.RegisterRoute("courseList", typeof(Views.CourseListPage));
        Routing.RegisterRoute("notifications", typeof(Views.NotificationsPage));
        System.Diagnostics.Debug.WriteLine("CAT_LOG: AppShell Constructor End");
    }
}
