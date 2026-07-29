/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get all tickets created by a user (Employee)

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketGetByCreatedBy
    @UserId     INT,
    @PageNumber INT = 1,
    @PageSize   INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Skip INT = (@PageNumber - 1) * @PageSize;

    SELECT 
        t.TicketId,
        t.TicketNumber,
        t.Subject,
        c.CategoryName,
        p.PriorityName,
        s.StatusName,
        CASE 
            WHEN t.AssignedTo IS NOT NULL 
            THEN ua.FirstName + ' ' + ua.LastName 
            ELSE 'Unassigned' 
        END AS AssignedToName,
        t.CreatedDate,
        t.ResolvedDate,
        t.ClosedDate
    FROM TicketResolverTicket t
    INNER JOIN TicketResolverTicketCategory c ON t.CategoryId = c.CategoryId
    INNER JOIN TicketResolverTicketPriority p ON t.PriorityId = p.PriorityId
    INNER JOIN TicketResolverTicketStatus s ON t.StatusId = s.StatusId
    LEFT JOIN TicketResolverUser ua ON t.AssignedTo = ua.UserId
    WHERE t.CreatedBy = @UserId AND t.IsActive = 1
    ORDER BY t.CreatedDate DESC
    OFFSET @Skip ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM TicketResolverTicket
    WHERE CreatedBy = @UserId AND IsActive = 1;
END
GO