
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Soft delete priority

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketPriorityDelete
    @PriorityId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverTicketPriority
        SET IsActive = 0,
            ModifiedDate = GETDATE()
        WHERE PriorityId = @PriorityId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
