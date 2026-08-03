CREATE   PROCEDURE TicketResolverTicketSearch
    @SearchTerm    NVARCHAR(200) = NULL,
    @CategoryId    INT = NULL,
    @PriorityId    INT = NULL,
    @StatusId      INT = NULL,
    @AssignedTo    INT = NULL,
    @CreatedBy     INT = NULL,
    @PageNumber    INT = 1,
    @PageSize      INT = 10,
    @SortColumn    NVARCHAR(50) = 'CreatedDate',
    @SortDirection VARCHAR(4) = 'DESC',
    @IsUnassigned  BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Skip INT = (@PageNumber - 1) * @PageSize;
    DECLARE @OrderBy NVARCHAR(100);
    DECLARE @Sql NVARCHAR(MAX);

    SET @OrderBy = CASE @SortColumn
        WHEN 'TicketNumber' THEN 't.TicketNumber'
        WHEN 'Subject'      THEN 't.Subject'
        WHEN 'Priority'     THEN 'p.Sequence'
        WHEN 'Status'       THEN 's.StatusName'
        ELSE 't.CreatedDate'
    END;

    SET @SortDirection = CASE WHEN UPPER(@SortDirection) = 'ASC' THEN 'ASC' ELSE 'DESC' END;

    SET @Sql = N'
        SELECT 
            t.TicketId,
            t.TicketNumber,
            t.Subject,
            c.CategoryName,
            p.PriorityName,
            p.Sequence AS PrioritySequence,
            s.StatusName,
            s.StatusId AS StatusIdVal,
            uc.FirstName + '' '' + uc.LastName AS CreatedByName,
            CASE 
                WHEN t.AssignedTo IS NOT NULL 
                THEN ua.FirstName + '' '' + ua.LastName 
                ELSE ''Unassigned'' 
            END AS AssignedToName,
            t.CreatedDate,
            t.ResolvedDate,
            t.ClosedDate,
            COUNT(*) OVER() AS TotalCount
        FROM TicketResolverTicket t
        INNER JOIN TicketResolverTicketCategory c ON t.CategoryId = c.CategoryId
        INNER JOIN TicketResolverTicketPriority p ON t.PriorityId = p.PriorityId
        INNER JOIN TicketResolverTicketStatus s ON t.StatusId = s.StatusId
        INNER JOIN TicketResolverUser uc ON t.CreatedBy = uc.UserId
        LEFT JOIN TicketResolverUser ua ON t.AssignedTo = ua.UserId
        WHERE 
            t.IsActive = 1
            AND (@SearchTerm IS NULL OR 
                 t.TicketNumber LIKE ''%'' + @SearchTerm + ''%'' OR 
                 t.Subject LIKE ''%'' + @SearchTerm + ''%'')
            AND (@CategoryId IS NULL OR t.CategoryId = @CategoryId)
            AND (@PriorityId IS NULL OR t.PriorityId = @PriorityId)
            AND (@StatusId IS NULL OR t.StatusId = @StatusId)
            AND (@AssignedTo IS NULL OR t.AssignedTo = @AssignedTo)
            AND (@CreatedBy IS NULL OR t.CreatedBy = @CreatedBy)
            AND (@IsUnassigned IS NULL OR (@IsUnassigned = 1 AND t.AssignedTo IS NULL))
        ORDER BY ' + @OrderBy + ' ' + @SortDirection + N'
        OFFSET @Skip ROWS
        FETCH NEXT @PageSize ROWS ONLY;';

    EXEC sp_executesql @Sql,
        N'@SearchTerm NVARCHAR(200), @CategoryId INT, @PriorityId INT, @StatusId INT, @AssignedTo INT, @CreatedBy INT, @Skip INT, @PageSize INT, @IsUnassigned BIT',
        @SearchTerm, @CategoryId, @PriorityId, @StatusId, @AssignedTo, @CreatedBy, @Skip, @PageSize, @IsUnassigned;
END
