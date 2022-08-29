using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.Models
{
    public class ProductDisplayModel : ObservableObject
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal RetailPrice { get; set; }
        private int _quantityInStock;
        public int QuantityInStock 
        { 
            get => _quantityInStock; 
            set => SetProperty(ref _quantityInStock, value); 
        }
        public bool IsTaxable { get; set; }
    }
}
