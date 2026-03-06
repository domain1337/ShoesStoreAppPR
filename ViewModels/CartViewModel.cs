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

        private ObservableCollection<PickupPoint> _pickupPoints;
        public ObservableCollection<PickupPoint> PickupPoints
        {
            get => _pickupPoints;
            set => Set(ref _pickupPoints, value);
        }

        private PickupPoint _selectedPickupPoint;
        public PickupPoint SelectedPickupPoint
        {
            get => _selectedPickupPoint;
            set => Set(ref _selectedPickupPoint, value);
        }

        public RelayCommand RemoveItemCommand { get; }
        public RelayCommand CheckoutCommand { get; }

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

            CheckoutCommand = new RelayCommand(async _ => await ExecuteCheckout(), _ => CartItems.Count > 0);

            LoadPickupPoints();
        }

        private async void LoadPickupPoints()
        {
            try
            {
                var response = await SupabaseService.Client.From<PickupPoint>().Get();
                PickupPoints = new ObservableCollection<PickupPoint>(response.Models);
            }
            catch { }
        }

        private async Task ExecuteCheckout()
        {
            if (!Services.UserService.IsAuthenticated)
            {
                NotificationService.Show("Оформление заказа доступно только авторизованным пользователям!", true);
                return;
            }

            if (SelectedPickupPoint == null)
            {
                NotificationService.Show("Пожалуйста, выберите пункт выдачи!", true);
                return;
            }

            if (CartItems == null || CartItems.Count == 0)
            {
                NotificationService.Show("Ваша корзина пуста!", true);
                return;
            }

            try
            {
                string productsInfo = string.Join(", ", CartItems.Select(p => p.Title));

                var newOrder = new Models.Order
                {
                    Id = Guid.NewGuid(),
                    CustomerEmail = Services.UserService.UserEmail,
                    OrderContent = $"[Пункт выдачи: {SelectedPickupPoint.FullAddress}] | Товары: {productsInfo}",
                    TotalPrice = TotalPrice,
                    Status = "Новый"
                };

                await Services.SupabaseService.Client.From<Models.Order>().Insert(newOrder);

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