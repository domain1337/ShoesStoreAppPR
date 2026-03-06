using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoesStoreApp.Views.Windows;
using System.Windows;

namespace ShoesStoreApp.Services
{
    public static class NotificationService
    {
        public static void Show(string message, bool isError = false)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var toast = new ToastWindow(message, isError);
                toast.Show();
            });
        }
    }
}
