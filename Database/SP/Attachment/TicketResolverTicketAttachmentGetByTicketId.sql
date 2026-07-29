/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get attachments for a ticket

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketAttachmentGetByTicketId
    @TicketId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        a.AttachmentId,
        a.TicketId,
        a.CommentId,
        a.OriginalFileName,
        a.StoredFileName,
        u.FirstName + ' ' + u.LastName AS UploadedByName,
        a.CreatedDate
    FROM TicketResolverTicketAttachment a
    INNER JOIN TicketResolverUser u ON a.CreatedBy = u.UserId
    WHERE a.TicketId = @TicketId AND a.IsActive = 1
    ORDER BY a.CreatedDate DESC;
END
GO