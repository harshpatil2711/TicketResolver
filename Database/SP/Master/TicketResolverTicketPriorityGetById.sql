/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get priority by ID

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketPriorityGetById
    @PriorityId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        PriorityId,
        PriorityName,
        Sequence
    FROM TicketResolverTicketPriority
    WHERE PriorityId = @PriorityId AND IsActive = 1;
END
GO