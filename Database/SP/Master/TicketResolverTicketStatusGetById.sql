
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get status by ID

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketStatusGetById
    @StatusId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        StatusId,
        StatusName,
        IsTerminalState
    FROM TicketResolverTicketStatus
    WHERE StatusId = @StatusId AND IsActive = 1;
END
