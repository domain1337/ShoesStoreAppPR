using Microsoft.Win32;
using System.IO;
using ShoesStoreApp.Models;
using ShoesStoreApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShoesStoreApp.ViewModels.Base
{
    public class ProductEditViewModel : ViewModelBase
    {
        public Product CurrentProduct { get; set; }
        public AsyncRelayCommand SaveCommand { get; set; }

        public AsyncRelayCommand SelectImageCommand { get; }

        public ProductEditViewModel(Product product)
        {
            CurrentProduct = new Product
            {
                Id = product.Id, Article = product.Article, Title = product.Title,
                Unit = product.Unit, Price = product.Price, Supplier = product.Supplier,
                Manufacturer = product.Manufacturer, Category = product.Category,
                Discount = product.Discount, QuantityInStock = product.QuantityInStock,
                Description = product.Description, ImagePath = product.ImagePath
            };
            SelectImageCommand = new AsyncRelayCommand(_ => ExecuteSelectImageAsync());
            SaveCommand = new AsyncRelayCommand(async _ => await Save());
        }

        private async Task Save()
        {
            try
            {
                var validation = StoreValidation.Product(CurrentProduct);
                if (validation != null)
                {
                    NotificationService.Show(validation, true);
                    return;
                }
                if (CurrentProduct.Id == Guid.Empty) CurrentProduct.Id = Guid.NewGuid();
                await StoreRepository.Current.SaveProductAsync(CurrentProduct);

                NotificationService.Show("Данные сохранены!");

                foreach (Window win in Application.Current.Windows)
                {
                    if (win.DataContext == this) win.DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                NotificationService.Show("Ошибка сохранения: " + ex.Message, true);
            }
        }
        private async Task ExecuteSelectImageAsync()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
                    CurrentProduct.ImagePath = await StoreRepository.Current.UploadProductImageAsync(Path.GetFileName(filePath), fileBytes);
                    OnPropertyChanged(nameof(CurrentProduct));
                }
                catch (Exception ex)
                {
                    NotificationService.Show("Ошибка загрузки фото: " + ex.Message, true);
                }
            }
        }
    }
}
