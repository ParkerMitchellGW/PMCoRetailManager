CREATE PROCEDURE [dbo].[spUserLookup]
	@Id nvarchar(128)
AS
	set nocount on;

	SELECT Id, FirstName, LastName, EmailAddress, CreatedDate from dbo.[User]
	where Id = @Id
RETURN 0
