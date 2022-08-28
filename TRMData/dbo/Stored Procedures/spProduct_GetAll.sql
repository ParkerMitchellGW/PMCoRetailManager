CREATE PROCEDURE [dbo].[spProduct_GetALl]
AS
begin
	set nocount on;

	SELECT [Id], [ProductName], [Description], [RetailPrice], [QuantityInStock], [IsTaxable]
	from dbo.Product
	order by ProductName;
end
