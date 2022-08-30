using Microsoft.AspNet.Identity;
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
    [Authorize]
    public class SaleController : ApiController
    {
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData();
            string userId = RequestContext.Principal.Identity.GetUserId();

            data.SaveSale(sale, userId);
        }

        [Authorize(Roles = "Admin, Manager")]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            if (RequestContext.Principal.IsInRole("Admin"))
            {
                // Do admin stuff
            }
            SaleData data = new SaleData();
            return data.GetSaleReport();
        }

        //public List<ProductModel> Get()
        //{
        //    ProductData data = new ProductData();

        //    return data.GetProducts();
        //}
    }
}