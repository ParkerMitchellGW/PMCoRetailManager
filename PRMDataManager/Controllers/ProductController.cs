using PRMDataManager.Library.DataAccess;
using PRMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PRMDataManager.Controllers
{
    // Example to add multiple roles
    // [Authorize(Roles = "Cashier,Admin")]
    [Authorize(Roles = "Cashier")]
    public class ProductController : ApiController
    {
        // GET api/<controller>
        public List<ProductModel> Get()
        {
            ProductData data = new ProductData();

            return data.GetProducts();
        }
    }
}