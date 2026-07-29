/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Soft delete attachment

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketAttachmentDelete
    @AttachmentId INT,
    @ModifiedBy   INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverTicketAttachment
        SET IsActive = 0,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = GETDATE()
        WHERE AttachmentId = @AttachmentId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO