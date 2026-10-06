using ShoesStoreApp.Services;
using ShoesStoreApp.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ShoesStoreApp.ViewModels.Base
{
    public class LoginViewModel : ViewModelBase
    {
        public AsyncRelayCommand LoginCommand { get; set; }
        public RelayCommand GuestCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        public LoginViewModel()
        {
            LoginCommand = new AsyncRelayCommand(async (p) =>
            {
                var passwordBox = p as PasswordBox;
                string password = passwordBox?.Password ?? string.Empty;
                await Login(password);
            });

            GuestCommand = new RelayCommand(_ => OpenMain("guest"));

            RegisterCommand = new RelayCommand(_ =>
            {
                var regWin = new RegisterWindow();
                regWin.ShowDialog();
            });
        }

        private async Task Login(string password)
        {
            var validation = StoreValidation.Account(Email, password, false);
            if (validation != null)
            {
                NotificationService.Show(validation, true);
                return;
            }

            try
            {
                var account = await AuthService.SignInAsync(Email.Trim(), password);

                if (!string.IsNullOrWhiteSpace(account.Email))
                {
                    UserService.SetAuthenticated(account.Email, account.Role);

                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is Views.Windows.LoginWindow) window.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid login credentials") || ex.Message.Contains("400"))
                {
                    NotificationService.Show("Неверный логин или пароль", true);
                }

                else if (ex.Message.Contains("SSL") || ex.Message.Contains("connection"))
                {
                    NotificationService.Show("Ошибка соединения с сервером.", true);
                }
                else
                {

                    NotificationService.Show($"Произошла ошибка: {ex.Message}", true);
                }
            }
        }

        private void OpenMain(string role)
        {
            if (role == "guest")
            {
                UserService.Reset();
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is LoginWindow) window.Close();
            }
        }

    }
}
