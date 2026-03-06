using ShoesStoreApp.Models;
using ShoesStoreApp.Services;
using ShoesStoreApp.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShoesStoreApp.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set => Set(ref _orders, value);
        }

        public OrderViewModel()
        {
            LoadOrders();
        }

        public async void LoadOrders()
        {
            try
            {
                var response = await SupabaseService.Client.From<Order>().Get();
                Orders = new ObservableCollection<Order>(response.Models);
                OnPropertyChanged(nameof(Orders)); 
            }
            catch (Exception ex)
            {
                NotificationService.Show("Ошибка при получении заказов: " + ex.Message, true);
            }
        }
    }
}
