using ShoesStoreApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ShoesStoreApp.ViewModels.Base
{
    public class RegisterViewModel : ViewModelBase
    {
        private string _email;
        public string Email { get => _email; set => Set(ref _email, value); }

        public RelayCommand RegisterCommand { get; }
        public RelayCommand BackCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(async (param) =>
            {
                var passwordBox = param as PasswordBox;
                string password = passwordBox?.Password;

                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(password))
                {
                    NotificationService.Show("Заполните все поля!", true);
                    return;
                }

                try
                {
                    var session = await SupabaseService.Client.Auth.SignUp(Email, password);

                    if (session != null)
                    {
                        NotificationService.Show("Регистрация успешна! Теперь вы можете войти.", true);
                        CloseWindow();
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Show("Ошибка регистрации: " + ex.Message, true);
                }
            });

            BackCommand = new RelayCommand(_ => CloseWindow());
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this) window.Close();
            }
        }
    }
}
