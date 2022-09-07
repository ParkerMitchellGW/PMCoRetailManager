using Microsoft.Extensions.Configuration;
using PRMDataManager.Library.Internal;
using PRMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDataManager.Library.DataAccess
{
    public class SaleData
    {
        private readonly IConfiguration _config;

        public SaleData(IConfiguration config)
        {
            _config = config;
        }
        public List<SaleReportModel> GetSaleReport()
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_Report", new { }, "PRMData");

            return output;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            //TODO: Make this SOLID/Dry/Better

            // Start filling in the models we will save to the database
            ProductData products = new ProductData(_config);
            var taxRate = ConfigHelper.GetTaxRate() / 100;

            // Fill in the available information
            List<SaleDetailDBModel> details = saleInfo.SaleDetails
                .Select(saleDetail =>
                {
                    var productInfo = products.GetProductById(saleDetail.ProductId);
                    if (productInfo == null) throw new Exception($"The product Id of {saleDetail.ProductId} could not be found in the database.");
                    decimal tax = productInfo.IsTaxable ?
                        productInfo.RetailPrice * saleDetail.Quantity * taxRate :
                        0;
                    return new SaleDetailDBModel
                    {
                        ProductId = saleDetail.ProductId,
                        Quantity = saleDetail.Quantity,
                        PurchasePrice = productInfo.RetailPrice * saleDetail.Quantity,
                        Tax = tax
                    };
                }).ToList();

            // Create the sale model
            SaleDBModel sale = new SaleDBModel
            {
                CashierId = cashierId,
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
            };
            sale.Total = sale.SubTotal + sale.Tax;

            // Save the sale model
            using SqlDataAccess sql = new SqlDataAccess(_config);
            try
            {
                sql.StartTransaction("PRMData");

                sql.SaveDataInTransaction("dbo.spSale_Insert", sale, "PRMData");

                // Get the ID from the sale model
                sale.Id =
                    sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { sale.CashierId, sale.SaleDate }, "PRMData").FirstOrDefault();

                // Finish filling in the sale detail models
                details.ForEach(detail =>
                {
                    detail.SaleId = sale.Id;

                    // Save the sale detail models
                    sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", detail, "PRMData");
                });

                sql.CommitTransaction();
            }
            catch
            {
                sql.RollbackTransaction();
                throw;
            }
        }
    }
}
