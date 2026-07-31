
-- ============================================================
-- SECTION: Log
-- ============================================================

/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Insert log entry

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverLogInsert
    @LogLevel   VARCHAR(20),
    @Source     VARCHAR(200) = NULL,
    @Message    NVARCHAR(MAX),
    @Exception  NVARCHAR(MAX) = NULL,
    @StackTrace NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO TicketResolverLog 
            (LogLevel, Source, Message, Exception, StackTrace)
        VALUES 
            (@LogLevel, @Source, @Message, @Exception, @StackTrace);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
