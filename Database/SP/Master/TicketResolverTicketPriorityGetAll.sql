/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get all active priorities

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketPriorityGetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        PriorityId,
        PriorityName,
        Sequence
    FROM TicketResolverTicketPriority
    WHERE IsActive = 1
    ORDER BY Sequence;
END
GO