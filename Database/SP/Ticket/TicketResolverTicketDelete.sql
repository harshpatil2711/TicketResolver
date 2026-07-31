
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Soft delete ticket

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketDelete
    @TicketId   INT,
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverTicket
        SET IsActive = 0,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = GETDATE()
        WHERE TicketId = @TicketId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
