using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRMDataManager.Library.DataAccess;
using PRMDataManager.Library.Models;

namespace PRMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryData _inventoryData;

        public InventoryController(IInventoryData inventoryData)
        {
            _inventoryData = inventoryData;
        }
        // Example of multiples roles being authorized
        // Anyone with any of these roles can use this function
        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public List<InventoryModel> GetInventory()
        {
            return _inventoryData.GetInventory();
        }

        // Example of needing multiple roles in order to authorize
        // Must be ALL of these roles to use this function
        //[Authorize(Roles = "WarehouseWorker")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void Post(InventoryModel item)
        {
            _inventoryData.SaveInventoryRecord(item);
        }
    }
}
