using ShoesStoreApp.Models;
using ShoesStoreApp.ViewModels.Base;
using ShoesStoreApp.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;

namespace ShoesStoreApp.ViewModels
{
    public class MainViewModel : Base.ViewModelBase
    {
        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }
        public Base.RelayCommand ShowProductsCommand { get; }
        public Base.RelayCommand ShowOrdersCommand { get; }
        public RelayCommand ShowCartCommand { get; }

        public bool IsManagerOrAdmin => Services.UserService.IsManagerOrAdmin;

        public bool IsNotGuest => Services.UserService.IsAuthenticated;

        public string WindowTitle => $"ООО «Обувь» - {Services.UserService.CurrentRole.ToUpper()}";

        public MainViewModel()
        {
            ShowProductsCommand = new Base.RelayCommand(_ => {
                if (Services.UserService.IsManagerOrAdmin)
                    CurrentPage = new AdminViewModel();
                else
                    CurrentPage = new ProductViewModel();
            });

            ShowOrdersCommand = new Base.RelayCommand(_ => 
            {
                CurrentPage = new OrderViewModel();
            });
            ShowCartCommand = new Base.RelayCommand(_ => {
                CurrentPage = new CartViewModel();
            });
            if (Services.UserService.IsManagerOrAdmin)
                CurrentPage = new AdminViewModel();
            else
                CurrentPage = new ProductViewModel();
        }
    }
}