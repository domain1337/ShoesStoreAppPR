using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Threading.Tasks;
using ShoesStoreApp.Models;
using ShoesStoreApp.Services;
using ShoesStoreApp.ViewModels.Base;

namespace ShoesStoreApp.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        private ObservableCollection<Product> _allProducts = new();

        private ICollectionView _productsView = null!;
        public ICollectionView ProductsView
        {
            get => _productsView;
            set => Set(ref _productsView, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { Set(ref _searchText, value); ProductsView?.Refresh(); }
        }

        public bool IsAdmin => UserService.IsAdmin;
        public bool IsManagerOrAdmin => UserService.IsManagerOrAdmin;
        public bool IsNotGuest => UserService.IsAuthenticated;

        public RelayCommand AddToCartCommand { get; }

        private string _sortType = "None";
        public string SortType
        {
            get => _sortType;
            set
            {
                if (Set(ref _sortType, value))
                {
                    ApplySorting();
                }
            }
        }

        public ProductViewModel()
        {
            AddToCartCommand = new RelayCommand(obj =>
            {
                if (obj is Product product)
                {
                    if (!UserService.IsAuthenticated)
                    {
                        NotificationService.Show("Войдите, чтобы добавить товар в корзину.", true);
                        return;
                    }
                    CartService.Add(product);

                    NotificationService.Show($"Товар '{product.Title}' добавлен в корзину!", false);
                }
            });

            _ = InitializeAsync();
        }

        private async Task InitializeAsync() => await LoadProductsAsync();

        public async Task LoadProductsAsync()
        {
            try
            {
                var products = await StoreRepository.Current.GetProductsAsync();
                _allProducts = new ObservableCollection<Product>(products);

                ProductsView = CollectionViewSource.GetDefaultView(_allProducts);

                ProductsView.Filter = (obj) =>
                {
                    if (!IsManagerOrAdmin) return true;
                    if (string.IsNullOrWhiteSpace(SearchText)) return true;

                    return ((Product)obj).Title?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true;
                };

                ApplySorting();
                OnPropertyChanged(nameof(ProductsView));
            }
            catch (Exception ex)
            {
                NotificationService.Show("Ошибка при загрузке товаров: " + ex.Message, true);
            }
        }
        public void ApplySorting()
        {
            if (ProductsView == null) return;

            ProductsView.SortDescriptions.Clear();

            if (SortType == "Asc")
            {
                ProductsView.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Ascending));
            }
            else if (SortType == "Desc")
            {
                ProductsView.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Descending));
            }

            ProductsView.Refresh();
        }
    }
}
