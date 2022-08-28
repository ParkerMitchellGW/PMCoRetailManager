using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.Library.Models
{
    public partial class CartItemModel : ObservableObject
    {
        public ProductModel Product { get; set; }

        private int _quantityInCart;
        public int QuantityInCart
        {
            get => _quantityInCart;
            set
            {
                SetProperty(ref _quantityInCart, value);
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public string DisplayText => $"{Product.ProductName} ({QuantityInCart})";
    }
}
