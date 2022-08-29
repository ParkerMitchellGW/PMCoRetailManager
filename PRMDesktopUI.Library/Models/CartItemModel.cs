using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.Library.Models
{
    public partial class CartItemModel
    {
        public ProductModel Product { get; set; }

        public int QuantityInCart { get; set; }
    }
}
