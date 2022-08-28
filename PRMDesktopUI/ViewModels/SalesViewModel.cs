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
        [NotifyCanExecuteChangedFor(nameof(AddToCartCommand))]
        private ProductModel? _selectedProduct;

        [ObservableProperty]
        private BindingList<CartItemModel> _cart = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddToCartCommand))]
        private int _itemQuantity = 1;

        private readonly ILoggedInUserModel _loggedInUser;

        private readonly IProductEndpoint _productEndpoint;

        public string SubTotal
        {
            get
            {
                decimal subTotal = Cart.Sum(item => item.QuantityInCart * item.Product.RetailPrice);
                return subTotal.ToString("C");
            }
        }
        public string Tax => "$0.00";
        public string Total => "$0.00";

        private bool CanAddToCart =>
            ItemQuantity >= 0 &&
            SelectedProduct?.QuantityInStock >= ItemQuantity;

        [RelayCommand(CanExecute = nameof(CanAddToCart))]
        private void AddToCart()
        {
            // Check to see if this item exists already
            CartItemModel? item = Cart.FirstOrDefault(item => item.Product == SelectedProduct);
            if(item is not null)
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
            OnPropertyChanged(nameof(SubTotal));
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
