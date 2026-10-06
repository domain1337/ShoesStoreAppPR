using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ShoesStoreApp.Models;
using ShoesStoreApp.Services;
using ShoesStoreApp.ViewModels.Base;

namespace ShoesStoreApp.ViewModels
{
    public class CartViewModel : ViewModelBase
    {
        public ObservableCollection<Product> CartItems => CartService.Items;

        public decimal TotalPrice => CartItems.Sum(p => p.FinalPrice);

        private ObservableCollection<PickupPoint> _pickupPoints = new();
        public ObservableCollection<PickupPoint> PickupPoints
        {
            get => _pickupPoints;
            set => Set(ref _pickupPoints, value);
        }

        private PickupPoint? _selectedPickupPoint;
        public PickupPoint? SelectedPickupPoint
        {
            get => _selectedPickupPoint;
            set => Set(ref _selectedPickupPoint, value);
        }

        public RelayCommand RemoveItemCommand { get; }
        public AsyncRelayCommand CheckoutCommand { get; }

        public CartViewModel()
        {
            RemoveItemCommand = new RelayCommand(obj =>
            {
                if (obj is Product product)
                {
                    CartService.Items.Remove(product);
                    OnPropertyChanged(nameof(TotalPrice)); 
                    NotificationService.Show("Товар удален из корзины");
                }
            });

            CheckoutCommand = new AsyncRelayCommand(async _ => await ExecuteCheckout(), _ => CartItems.Count > 0);

            _ = LoadPickupPointsAsync();
        }

        public async Task LoadPickupPointsAsync()
        {
            try
            {
                var points = await StoreRepository.Current.GetPickupPointsAsync();
                PickupPoints = new ObservableCollection<PickupPoint>(points);
            }
            catch (Exception ex)
            {
                NotificationService.Show("Ошибка загрузки пунктов выдачи: " + ex.Message, true);
            }
        }

        private async Task ExecuteCheckout()
        {
            var validation = StoreValidation.Checkout(UserService.IsAuthenticated, CartItems.Count, SelectedPickupPoint);
            if (validation != null)
            {
                NotificationService.Show(validation, true);
                return;
            }

            try
            {
                string productsInfo = string.Join(", ", CartItems.Select(p => p.Title));

                var newOrder = new Models.Order
                {
                    Id = Guid.NewGuid(),
                    CustomerEmail = Services.UserService.UserEmail ?? string.Empty,
                    OrderContent = $"[Пункт выдачи: {SelectedPickupPoint!.FullAddress}] | Товары: {productsInfo}",
                    TotalPrice = TotalPrice,
                    Status = "Новый"
                };

                await StoreRepository.Current.CreateOrderAsync(newOrder);

                NotificationService.Show("Заказ успешно оформлен! Ожидайте уведомления.");

                Services.CartService.Clear();

                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(CartItems));
            }
            catch (Exception ex)
            {
                NotificationService.Show("Ошибка при оформлении заказа: " + ex.Message, true);
            }
        }
    }
}
