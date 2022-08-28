using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PRMDesktopUI.Library.Api;
using PRMDesktopUI.Library.Helpers;
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
        [NotifyCanExecuteChangedFor(nameof(AddToCartCommand))]
        private ProductModel? _selectedProduct;

        [ObservableProperty]
        private BindingList<CartItemModel> _cart = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddToCartCommand))]
        private int _itemQuantity = 1;

        private readonly ILoggedInUserModel _loggedInUser;

        private readonly IProductEndpoint _productEndpoint;
        private readonly IConfigHelper _config;

        public string SubTotal => CalculateSubTotal().ToString("C");

        private decimal CalculateSubTotal() => Cart.Sum(item => item.QuantityInCart * item.Product.RetailPrice);
        public string Tax => CalculateTax().ToString("C");
        private decimal CalculateTax()
        {
            decimal taxRate = _config.GetTaxRate() / 100;
            return Cart.
                Where(item => item.Product.IsTaxable).
                Sum(item => item.QuantityInCart * item.Product.RetailPrice * taxRate);
        }
        public string Total => (CalculateSubTotal() + CalculateTax()).ToString("C");

        private bool CanAddToCart =>
            ItemQuantity >= 0 &&
            SelectedProduct?.QuantityInStock >= ItemQuantity;

        [RelayCommand(CanExecute = nameof(CanAddToCart))]
        private void AddToCart()
        {
            // Check to see if this item exists already
            CartItemModel? item = Cart.FirstOrDefault(item => item.Product == SelectedProduct);
            if (item is not null)
            {
                item.QuantityInCart += ItemQuantity;
            }
            else
            {
                item = new()
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }

            SelectedProduct!.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            OnCartChanged();
        }

        private void OnCartChanged()
        {
            OnPropertyChanged(nameof(SubTotal));
            OnPropertyChanged(nameof(Tax));
            OnPropertyChanged(nameof(Total));
            AddToCartCommand.NotifyCanExecuteChanged();
        }

        private bool CanRemoveFromCart
        {
            // Verify we can remove from cart
            get => false;
        }
        [RelayCommand(CanExecute = nameof(CanRemoveFromCart))]
        private void RemoveFromCart()
        {
            OnCartChanged();
        }
        private bool CanCheckOut
        {
            get => false;
        }

        [RelayCommand(CanExecute = nameof(CanCheckOut))]
        private void CheckOut()
        {
        }

        public SalesViewModel(ILoggedInUserModel loggedInUser, IProductEndpoint productEndpoint, IConfigHelper config)
        {
            _loggedInUser = loggedInUser;
            _productEndpoint = productEndpoint;
            _config = config;
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
