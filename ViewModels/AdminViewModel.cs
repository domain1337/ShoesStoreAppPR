using System;
using System.Windows;
using System.Threading.Tasks;
using ShoesStoreApp.Models;
using ShoesStoreApp.Services;
using ShoesStoreApp.ViewModels.Base;
using ShoesStoreApp.Views.Windows;

namespace ShoesStoreApp.ViewModels
{
    public class AdminViewModel : ProductViewModel
    {
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public AdminViewModel()
        {
            AddCommand = new RelayCommand(_ => ExecuteAdd());

            EditCommand = new RelayCommand(obj =>
            {
                if (obj is Product product) ExecuteEdit(product);
            });

            DeleteCommand = new RelayCommand(async obj =>
            {
                if (obj is Product product) await ExecuteDelete(product);
            });
        }

        private void ExecuteAdd()
        {
            var newProduct = new Product();
            OpenEditor(newProduct);
        }

        private void ExecuteEdit(Product product)
        {
            OpenEditor(product);
        }
        private void OpenEditor(Product product)
        {
            var editWindow = new ProductEditWindow();
            var vm = new ProductEditViewModel(product);
            editWindow.DataContext = vm;

            if (editWindow.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private async Task ExecuteDelete(Product product)
        {
            var result = MessageBox.Show($"Вы уверены, что хотите удалить '{product.Title}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await SupabaseService.Client
                        .From<Product>()
                        .Where(x => x.Id == product.Id)
                        .Delete();

                    LoadProducts();
                }
                catch (Exception ex)
                {
                    NotificationService.Show("Ошибка при удалении: " + ex.Message, true);
                }
            }
        }
    }
}