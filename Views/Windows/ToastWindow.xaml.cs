using System;
using System.Windows;
using System.Windows.Media;

namespace ShoesStoreApp.Views.Windows
{
    public partial class ToastWindow : Window
    {
        public ToastWindow(string message, bool isError = false)
        {
            InitializeComponent();
            MessageText.Text = message;

            if (isError)
            {
                ToastBorder.BorderBrush = Brushes.Red;
                IconText.Text = "!";
            }

            Window parent = Application.Current.MainWindow;
            if (parent != null && parent.IsVisible)
            {
                this.Left = parent.Left + parent.ActualWidth - this.Width - 20;
                this.Top = parent.Top + parent.ActualHeight - this.Height - 20;
            }
            else
            {
                var desktopWorkingArea = SystemParameters.WorkArea;
                this.Left = desktopWorkingArea.Right - this.Width - 10;
                this.Top = desktopWorkingArea.Bottom - this.Height - 10;
            }

            AutoClose();
        }

        private async void AutoClose()
        {
            await System.Threading.Tasks.Task.Delay(3500);
            this.Close();
        }
    }
}