/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Soft delete status

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketStatusDelete
    @StatusId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverTicketStatus
        SET IsActive = 0,
            ModifiedDate = GETDATE()
        WHERE StatusId = @StatusId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO