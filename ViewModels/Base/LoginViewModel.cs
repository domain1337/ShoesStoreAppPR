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
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand GuestCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }

        private string _email;
        public string Email
        {
            get => _email;
            set => Set(ref _email, value);
        }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async (p) =>
            {
                var passwordBox = p as PasswordBox;
                string password = passwordBox?.Password;
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
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
            {
                NotificationService.Show("Введите логин и пароль", true);
                return;
            }

            try
            {
                var session = await SupabaseService.Client.Auth.SignIn(Email, password);

                if (session != null && session.User != null)
                {
                    string role = "client";
                    var metadata = session.User.UserMetadata;
                    if (metadata != null && metadata.ContainsKey("role"))
                    {
                        role = metadata["role"].ToString().Trim('"', ' ');
                    }

                    Services.UserService.CurrentRole = role.ToLower();
                    Services.UserService.UserEmail = session.User.Email;

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
                UserService.CurrentRole = "guest";
                UserService.UserEmail = "Гость";
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
