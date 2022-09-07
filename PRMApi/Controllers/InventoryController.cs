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
        private readonly IConfiguration _config;

        public InventoryController(IConfiguration config)
        {
            _config = config;
        }
        // Example of multiples roles being authorized
        // Anyone with any of these roles can use this function
        [Authorize(Roles = "Manager,Admin")]
        public List<InventoryModel> GetInventory()
        {
            InventoryData data = new InventoryData(_config);
            return data.GetInventory();
        }

        // Example of needing multiple roles in order to authorize
        // Must be ALL of these roles to use this function
        //[Authorize(Roles = "WarehouseWorker")]
        [Authorize(Roles = "Admin")]
        public void Post(InventoryModel item)
        {
            InventoryData data = new InventoryData(_config);

            data.SaveInventoryRecord(item);
        }
    }
}
