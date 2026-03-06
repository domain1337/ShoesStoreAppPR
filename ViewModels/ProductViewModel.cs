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
        private ObservableCollection<Product> _allProducts;

        private ICollectionView _productsView;
        public ICollectionView ProductsView
        {
            get => _productsView;
            set => Set(ref _productsView, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { Set(ref _searchText, value); ProductsView?.Refresh(); }
        }

        public bool IsAdmin => UserService.IsAdmin;
        public bool IsManagerOrAdmin => UserService.IsManagerOrAdmin;

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
                    CartService.Add(product);

                    NotificationService.Show($"Товар '{product.Title}' добавлен в корзину!", false);
                }
            });

            LoadProducts();
        }

        public async void LoadProducts()
        {
            try
            {
                var response = await SupabaseService.Client.From<Product>().Get();
                _allProducts = new ObservableCollection<Product>(response.Models);

                ProductsView = CollectionViewSource.GetDefaultView(_allProducts);

                ProductsView.Filter = (obj) =>
                {
                    if (!IsManagerOrAdmin) return true;
                    if (string.IsNullOrWhiteSpace(SearchText)) return true;

                    return ((Product)obj).Title.ToLower().Contains(SearchText.ToLower());
                };

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