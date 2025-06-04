using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace Process_Explorer.GUI.Helpers
{
    public static class ToastNotificationHelper
    {   
       public static void ShowMessage(string title, string message)
       {
            var toast = new AppNotificationBuilder()
            .AddText(title)
            .AddText(message)
            .BuildNotification();

            AppNotificationManager.Default.Show(toast);
       }
    }
}
