using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PRMDesktopUI.Library.Api;
using PRMDesktopUI.Library.Helpers;
using PRMDesktopUI.Library.Models;
using PRMDesktopUI.Messages;
using PRMDesktopUI.Models;
using PRMDesktopUI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.ViewModels
{
    [ObservableObject]
    [ObservableRecipient]
    public partial class SalesViewModel : IReceiveViewEvents
    {
        [ObservableProperty]
        private BindingList<ProductDisplayModel> _products = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddToCartCommand))]
        private ProductDisplayModel? _selectedProduct;

        [ObservableProperty]
        private BindingList<CartItemDisplayModel> _cart = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveFromCartCommand))]
        private CartItemDisplayModel? _selectedCartItem;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddToCartCommand))]
        private int _itemQuantity = 1;

        private readonly ILoggedInUserModel _loggedInUser;

        private readonly IProductEndpoint _productEndpoint;
        private readonly IConfigHelper _config;
        private readonly ISaleEndpoint _saleEndpoint;
        private readonly IMapper _mapper;
        private readonly IStatusInfoDisplay _statusInfo;

        public SalesViewModel(ILoggedInUserModel loggedInUser, IProductEndpoint productEndpoint, IConfigHelper config,
            ISaleEndpoint saleEndpoint, IMapper mapper, IStatusInfoDisplay statusInfo)
        {
            _loggedInUser = loggedInUser;
            _productEndpoint = productEndpoint;
            _config = config;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
            _statusInfo = statusInfo;
            Messenger = WeakReferenceMessenger.Default;
        }
        private async Task ResetSalesViewModel()
        {
            ItemQuantity = 1;
            SelectedCartItem = null;
            SelectedProduct = null;
            Cart = new();
            await LoadProducts();

        }
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
            CartItemDisplayModel? item = Cart.FirstOrDefault(item => item.Product == SelectedProduct);
            if (item is not null)
            {
                item.QuantityInCart += ItemQuantity;
            }
            else
            {
                item = new()
                {
                    Product = SelectedProduct!,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }

            SelectedProduct!.QuantityInStock -= ItemQuantity;
            ItemQuantity = 1;
            OnCartChanged();
        }

        partial void OnCartChanged(BindingList<CartItemDisplayModel> value)
        {
            OnCartChanged();
        }

        private void OnCartChanged()
        {
            OnPropertyChanged(nameof(SubTotal));
            OnPropertyChanged(nameof(Tax));
            OnPropertyChanged(nameof(Total));
            AddToCartCommand.NotifyCanExecuteChanged();
            RemoveFromCartCommand.NotifyCanExecuteChanged();
            CheckOutCommand.NotifyCanExecuteChanged();
        }

        private bool CanRemoveFromCart => SelectedCartItem != null;
        [RelayCommand(CanExecute = nameof(CanRemoveFromCart))]
        private void RemoveFromCart()
        {
            CartItemDisplayModel cartItem = SelectedCartItem!;
            ProductDisplayModel? product = Products.FirstOrDefault(item => item == cartItem.Product);
            if (product is not null)
            {
                product.QuantityInStock += cartItem.QuantityInCart;
            }

            Cart.Remove(cartItem);
            SelectedCartItem = null;
            OnCartChanged();
        }
        private bool CanCheckOut => Cart?.Count > 0;

        [RelayCommand(CanExecute = nameof(CanCheckOut))]
        private async Task CheckOut()
        {
            SaleModel sale = new()
            {
                SaleDetails = Cart
                .Select(cartItem =>
                    new SaleDetailModel()
                    {
                        ProductId = cartItem.Product.Id,
                        Quantity = cartItem.QuantityInCart
                    })
                .ToList()
            };

            await _saleEndpoint.PostSale(sale);
            await ResetSalesViewModel();
        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAll();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }

        public async void OnViewLoaded(object view)
        {
            try
            {
                await LoadProducts();
            }
            catch (Exception ex)
            {
                if(ex.Message == "Unauthorized")
                {
                    _statusInfo.ShowMessage("You do not have permission to access this.", "Unauthorized", "System Error");
                }
                else
                {
                    _statusInfo.ShowMessage(ex.Message, "Fatal Exception");
                }
                Messenger.Send<ClosePageMessage>();
            }
        }
    }
}
