CREATE PROCEDURE [dbo].[spInventory_Insert]
    @ProductId INT,
    @Quantity INT,
    @PurchasePrice MONEY,
    @PurchaseDate DATETIME2
AS
begin
    set nocount on;
	
    insert into dbo.Inventory(ProductId, Quantity, PurchasePrice, PurchaseDate)
    values (@ProductId, @Quantity, @PurchasePrice, @PurchaseDate)
end