
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Update status

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketStatusUpdate
    @StatusId       INT,
    @StatusName     NVARCHAR(50),
    @IsTerminalState BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM TicketResolverTicketStatus WHERE StatusName = @StatusName AND StatusId != @StatusId AND IsActive = 1)
        BEGIN
            RAISERROR('Status name already exists.', 16, 1);
            RETURN;
        END

        UPDATE TicketResolverTicketStatus
        SET StatusName = @StatusName,
            IsTerminalState = @IsTerminalState,
            ModifiedDate = GETDATE()
        WHERE StatusId = @StatusId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
