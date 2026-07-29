/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get all active categories

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketCategoryGetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CategoryId,
        CategoryName
    FROM TicketResolverTicketCategory
    WHERE IsActive = 1
    ORDER BY CategoryName;
END
GO