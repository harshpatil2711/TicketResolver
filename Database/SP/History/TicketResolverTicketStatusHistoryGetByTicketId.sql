CREATE PROCEDURE TicketResolverTicketStatusHistoryGetByTicketId
    @TicketId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        h.HistoryId,
        h.TicketId,
        h.OldStatusId,
        os.StatusName AS OldStatusName,
        h.NewStatusId,
        ns.StatusName AS NewStatusName,
        h.PreviousAssignedTo,
        CASE 
            WHEN h.PreviousAssignedTo IS NOT NULL 
            THEN upa.FirstName + ' ' + upa.LastName 
            ELSE NULL 
        END AS PreviousAssignedToName,
        h.CurrentAssignedTo,
        CASE 
            WHEN h.CurrentAssignedTo IS NOT NULL 
            THEN uca.FirstName + ' ' + uca.LastName 
            ELSE NULL 
        END AS CurrentAssignedToName,
        h.ChangeReason,
        h.CreatedBy,
        u.FirstName + ' ' + u.LastName AS ChangedByName,
        h.CreatedDate
    FROM TicketResolverTicketStatusHistory h
    INNER JOIN TicketResolverTicketStatus os ON h.OldStatusId = os.StatusId
    INNER JOIN TicketResolverTicketStatus ns ON h.NewStatusId = ns.StatusId
    INNER JOIN TicketResolverUser u ON h.CreatedBy = u.UserId
    LEFT JOIN TicketResolverUser upa ON h.PreviousAssignedTo = upa.UserId
    LEFT JOIN TicketResolverUser uca ON h.CurrentAssignedTo = uca.UserId
    WHERE h.TicketId = @TicketId
    ORDER BY h.CreatedDate DESC;
END
