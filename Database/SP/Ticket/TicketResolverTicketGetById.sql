/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get ticket details by ID with joins

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketGetById
    @TicketId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.TicketId,
        t.TicketNumber,
        t.Subject,
        t.Description,
        t.CategoryId,
        c.CategoryName,
        t.PriorityId,
        p.PriorityName,
        p.Sequence AS PrioritySequence,
        t.StatusId,
        s.StatusName,
        s.IsTerminalState,
        t.CreatedBy,
        uc.FirstName + ' ' + uc.LastName AS CreatedByName,
        t.AssignedTo,
        CASE 
            WHEN t.AssignedTo IS NOT NULL 
            THEN ua.FirstName + ' ' + ua.LastName 
            ELSE NULL 
        END AS AssignedToName,
        t.CreatedDate,
        t.ResolvedDate,
        t.ClosedDate,
        t.ModifiedBy,
        t.ModifiedDate,
        t.IsActive
    FROM TicketResolverTicket t
    INNER JOIN TicketResolverTicketCategory c ON t.CategoryId = c.CategoryId
    INNER JOIN TicketResolverTicketPriority p ON t.PriorityId = p.PriorityId
    INNER JOIN TicketResolverTicketStatus s ON t.StatusId = s.StatusId
    INNER JOIN TicketResolverUser uc ON t.CreatedBy = uc.UserId
    LEFT JOIN TicketResolverUser ua ON t.AssignedTo = ua.UserId
    WHERE t.TicketId = @TicketId AND t.IsActive = 1;
END
GO