
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Insert new status

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketStatusInsert
    @StatusName     NVARCHAR(50),
    @IsTerminalState BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM TicketResolverTicketStatus WHERE StatusName = @StatusName AND IsActive = 1)
        BEGIN
            RAISERROR('Status already exists.', 16, 1);
            RETURN;
        END

        INSERT INTO TicketResolverTicketStatus (StatusName, IsTerminalState)
        VALUES (@StatusName, @IsTerminalState);

        SELECT SCOPE_IDENTITY() AS StatusId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
