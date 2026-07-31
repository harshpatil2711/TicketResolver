
-- ============================================================
-- SECTION: Comment
-- ============================================================

/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get all comments for a ticket

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketCommentGetByTicketId
    @TicketId INT,
    @UserId   INT,
    @RoleId   INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        tc.CommentId,
        tc.TicketId,
        tc.UserId,
        u.FirstName + ' ' + u.LastName AS UserName,
        r.RoleName,
        tc.CommentText,
        tc.IsInternalNote,
        tc.CreatedDate
    FROM TicketResolverTicketComment tc
    INNER JOIN TicketResolverUser u ON tc.UserId = u.UserId
    INNER JOIN TicketResolverRole r ON u.RoleId = r.RoleId
    WHERE tc.TicketId = @TicketId 
        AND tc.IsActive = 1
        AND (
            tc.IsInternalNote = 0
            OR @RoleId IN (1, 2)
            OR tc.UserId = @UserId
        )
    ORDER BY tc.CreatedDate ASC;
END
