/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get category by ID

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketCategoryGetById
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CategoryId,
        CategoryName
    FROM TicketResolverTicketCategory
    WHERE CategoryId = @CategoryId AND IsActive = 1;
END
GO