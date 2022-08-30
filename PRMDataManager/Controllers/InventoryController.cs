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
    public class InventoryController : ApiController
    {
        public List<InventoryModel> GetSalesReport()
        {
            InventoryData data = new InventoryData();
            return data.GetInventory();
        }

        public void Post(InventoryModel item)
        {
            InventoryData data = new InventoryData();

            data.SaveInventoryRecord(item);
        }
    }
}