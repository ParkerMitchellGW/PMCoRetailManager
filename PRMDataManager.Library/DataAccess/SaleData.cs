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
    public class SaleData : ISaleData
    {
        private readonly ISqlDataAccess _sql;
        private readonly IProductData _productData;

        public SaleData(ISqlDataAccess sql, IProductData productData)
        {
            _sql = sql;
            _productData = productData;
        }
        public List<SaleReportModel> GetSaleReport()
        {
            var output = _sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_Report", new { }, "PRMData");

            return output;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            //TODO: Make this SOLID/Dry/Better

            // Start filling in the models we will save to the database
            var taxRate = ConfigHelper.GetTaxRate() / 100;

            // Fill in the available information
            List<SaleDetailDBModel> details = saleInfo.SaleDetails
                .Select(saleDetail =>
                {
                    var productInfo = _productData.GetProductById(saleDetail.ProductId);
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
            SaleDBModel sale = new()
            {
                CashierId = cashierId,
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax),
            };
            sale.Total = sale.SubTotal + sale.Tax;

            // Save the sale model
            try
            {
                _sql.StartTransaction("PRMData");

                _sql.SaveDataInTransaction("dbo.spSale_Insert", sale, "PRMData");

                // Get the ID from the sale model
                sale.Id =
                    _sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { sale.CashierId, sale.SaleDate }, "PRMData").FirstOrDefault();

                // Finish filling in the sale detail models
                details.ForEach(detail =>
                {
                    detail.SaleId = sale.Id;

                    // Save the sale detail models
                    _sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", detail, "PRMData");
                });

                _sql.CommitTransaction();
            }
            catch
            {
                _sql.RollbackTransaction();
                throw;
            }
        }
    }
}
