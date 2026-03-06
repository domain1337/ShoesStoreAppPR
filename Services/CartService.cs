using ShoesStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesStoreApp.Services
{
    public static class CartService
    {
        public static ObservableCollection<Product> Items { get; } = new ObservableCollection<Product>();

        public static decimal TotalPrice => Items.Sum(p => p.FinalPrice);

        public static void Add(Product product)
        {
            Items.Add(product);
        }

        public static void Clear()
        {
            Items.Clear();
        }
    }
}
