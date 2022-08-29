CREATE PROCEDURE [dbo].[spSale_Lookup]
    @CashierId NVARCHAR(128),
    @SaleDate DATETIME2
AS
begin
    set nocount on;
	
    select Id from dbo.Sale where CashierId = @CashierId and SaleDate = @SaleDate;
end