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
        public RelayCommand SaveCommand { get; set; }

        public RelayCommand SelectImageCommand { get; }

        public ProductEditViewModel(Product product)
        {
            CurrentProduct = product;
            SelectImageCommand = new RelayCommand(_ => ExecuteSelectImage());
            SaveCommand = new RelayCommand(async _ => await Save());
        }

        private async Task Save()
        {
            try
            {
                await SupabaseService.Client.From<Product>().Upsert(CurrentProduct);

                NotificationService.Show("Данные сохранены!", true);

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
        private async void ExecuteSelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(filePath);
                    byte[] fileBytes = File.ReadAllBytes(filePath);

                    string storagePath = $"products/{Guid.NewGuid()}_{fileName}";
                    await SupabaseService.Client.Storage.From("images").Upload(fileBytes, storagePath);

                    string publicUrl = SupabaseService.Client.Storage.From("images").GetPublicUrl(storagePath);

                    CurrentProduct.ImagePath = publicUrl;
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
