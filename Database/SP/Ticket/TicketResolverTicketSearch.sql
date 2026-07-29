/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Search tickets with filters (Admin)

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketSearch
    @SearchTerm  NVARCHAR(200) = NULL,
    @CategoryId  INT = NULL,
    @PriorityId  INT = NULL,
    @StatusId    INT = NULL,
    @AssignedTo  INT = NULL,
    @CreatedBy   INT = NULL,
    @PageNumber  INT = 1,
    @PageSize    INT = 10
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
        p.Sequence AS PrioritySequence,
        s.StatusName,
        s.StatusId AS StatusIdVal,
        uc.FirstName + ' ' + uc.LastName AS CreatedByName,
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
    INNER JOIN TicketResolverUser uc ON t.CreatedBy = uc.UserId
    LEFT JOIN TicketResolverUser ua ON t.AssignedTo = ua.UserId
    WHERE 
        t.IsActive = 1
        AND (@SearchTerm IS NULL OR 
             t.TicketNumber LIKE '%' + @SearchTerm + '%' OR 
             t.Subject LIKE '%' + @SearchTerm + '%')
        AND (@CategoryId IS NULL OR t.CategoryId = @CategoryId)
        AND (@PriorityId IS NULL OR t.PriorityId = @PriorityId)
        AND (@StatusId IS NULL OR t.StatusId = @StatusId)
        AND (@AssignedTo IS NULL OR t.AssignedTo = @AssignedTo)
        AND (@CreatedBy IS NULL OR t.CreatedBy = @CreatedBy)
    ORDER BY t.CreatedDate DESC
    OFFSET @Skip ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM TicketResolverTicket t
    WHERE 
        t.IsActive = 1
        AND (@SearchTerm IS NULL OR 
             t.TicketNumber LIKE '%' + @SearchTerm + '%' OR 
             t.Subject LIKE '%' + @SearchTerm + '%')
        AND (@CategoryId IS NULL OR t.CategoryId = @CategoryId)
        AND (@PriorityId IS NULL OR t.PriorityId = @PriorityId)
        AND (@StatusId IS NULL OR t.StatusId = @StatusId)
        AND (@AssignedTo IS NULL OR t.AssignedTo = @AssignedTo)
        AND (@CreatedBy IS NULL OR t.CreatedBy = @CreatedBy);
END
GO