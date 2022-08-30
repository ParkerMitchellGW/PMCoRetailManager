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
        // Example of multiples roles being authorized
        // Anyone with any of these roles can use this function
        [Authorize(Roles = "Manager,Admin")]
        public List<InventoryModel> GetInventory()
        {
            InventoryData data = new InventoryData();
            return data.GetInventory();
        }

        // Example of needing multiple roles in order to authorize
        // Must be ALL of these roles to use this function
        //[Authorize(Roles = "WarehouseWorker")]
        [Authorize(Roles = "Admin")]
        public void Post(InventoryModel item)
        {
            InventoryData data = new InventoryData();

            data.SaveInventoryRecord(item);
        }
    }
}