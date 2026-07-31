
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get attachment by ID

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketAttachmentGetById
    @AttachmentId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        a.AttachmentId,
        a.TicketId,
        a.CommentId,
        a.OriginalFileName,
        a.StoredFileName,
        a.CreatedBy,
        a.CreatedDate
    FROM TicketResolverTicketAttachment a
    WHERE a.AttachmentId = @AttachmentId AND a.IsActive = 1;
END
