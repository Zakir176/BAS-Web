using System.Collections.ObjectModel;

namespace catv1.ViewModels;

public class NotificationItem
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Icon { get; set; } = "\ue7f4"; // Default notification icon
}

public class NotificationsViewModel : BaseViewModel
{
    public ObservableCollection<NotificationItem> Notifications { get; } = new();

    public NotificationsViewModel()
    {
        Title = "Notifications";
        LoadDummyNotifications();
    }

    private void LoadDummyNotifications()
    {
        Notifications.Add(new NotificationItem
        {
            Title = "Attendance Marked",
            Message = "You were marked present in CS101.",
            Time = "2 hours ago",
            Icon = "\ue86c" // check_circle
        });

        Notifications.Add(new NotificationItem
        {
            Title = "New Section Available",
            Message = "A new section for MA200 has been created.",
            Time = "1 day ago",
            Icon = "\ue88a" // info
        });
    }
}
