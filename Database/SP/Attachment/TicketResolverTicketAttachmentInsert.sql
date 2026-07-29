/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Insert attachment record

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketAttachmentInsert
    @TicketId         INT,
    @CommentId        INT = NULL,
    @OriginalFileName VARCHAR(255),
    @StoredFileName   VARCHAR(255),
    @CreatedBy        INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO TicketResolverTicketAttachment 
            (TicketId, CommentId, OriginalFileName, StoredFileName, CreatedBy)
        VALUES 
            (@TicketId, @CommentId, @OriginalFileName, @StoredFileName, @CreatedBy);

        SELECT SCOPE_IDENTITY() AS AttachmentId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO