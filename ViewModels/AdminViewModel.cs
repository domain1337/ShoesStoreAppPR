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
        public AsyncRelayCommand DeleteCommand { get; }

        public AdminViewModel()
        {
            AddCommand = new RelayCommand(_ => ExecuteAdd());

            EditCommand = new RelayCommand(obj =>
            {
                if (obj is Product product) ExecuteEdit(product);
            });

            DeleteCommand = new AsyncRelayCommand(async obj =>
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
                _ = LoadProductsAsync();
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
                    await StoreRepository.Current.DeleteProductAsync(product.Id);

                    await LoadProductsAsync();
                }
                catch (Exception ex)
                {
                    NotificationService.Show("Ошибка при удалении: " + ex.Message, true);
                }
            }
        }
    }
}
