using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PRMDesktopUI.Library.Api;
using PRMDesktopUI.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.ViewModels
{
    [ObservableObject]
    public partial class SalesViewModel : IReceiveViewEvents
    {
        [ObservableProperty]
        private BindingList<ProductModel> _products = new();

        [ObservableProperty]
        private BindingList<string> _cart = new();

        [ObservableProperty]
        private int _itemQuantity;
        private readonly ILoggedInUserModel _loggedInUser;
        private readonly IProductEndpoint _productEndpoint;

        public string SubTotal => "$0.00";
        public string Tax => "$0.00";
        public string Total => "$0.00";

        private bool CanAddToCart
        {
            // Verify we can add to cart
            get => false;
        }

        [RelayCommand(CanExecute = nameof(CanAddToCart))]
        private void AddToCart()
        {

        }

        private bool CanRemoveFromCart
        {
            // Verify we can remove from cart
            get => false;
        }
        [RelayCommand(CanExecute = nameof(CanRemoveFromCart))]
        private void RemoveFromCart()
        {

        }
        private bool CanCheckOut
        {
            get => false;
        }

        [RelayCommand(CanExecute = nameof(CanCheckOut))]
        private void CheckOut()
        {
        }

        public SalesViewModel(ILoggedInUserModel loggedInUser, IProductEndpoint productEndpoint)
        {
            _loggedInUser = loggedInUser;
            _productEndpoint = productEndpoint;
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            Products = new BindingList<ProductModel>(productList);
        }

        public async void OnViewLoaded(object view)
        {
            await LoadProducts();
        }
    }
}
