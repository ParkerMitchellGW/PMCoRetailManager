CREATE PROCEDURE [dbo].[spSaleDetail_Insert]
    @SaleId INT,
    @ProductId INT,
    @Quantity INT,
    @PurchasePrice MONEY,
    @Tax MONEY
AS
begin
	insert into dbo.SaleDetail(SaleId, ProductId, Quantity, PurchasePrice, Tax)
    values(@SaleId, @ProductId, @Quantity, @PurchasePrice, @Tax);
end