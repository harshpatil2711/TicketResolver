/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get all active statuses

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketStatusGetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        StatusId,
        StatusName,
        IsTerminalState
    FROM TicketResolverTicketStatus
    WHERE IsActive = 1
    ORDER BY StatusId;
END
GO