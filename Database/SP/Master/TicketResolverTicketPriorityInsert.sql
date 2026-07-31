
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Insert new priority

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketPriorityInsert
    @PriorityName NVARCHAR(50),
    @Sequence     INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM TicketResolverTicketPriority WHERE PriorityName = @PriorityName AND IsActive = 1)
        BEGIN
            RAISERROR('Priority already exists.', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM TicketResolverTicketPriority WHERE Sequence = @Sequence AND IsActive = 1)
        BEGIN
            RAISERROR('Priority sequence already exists.', 16, 1);
            RETURN;
        END

        INSERT INTO TicketResolverTicketPriority (PriorityName, Sequence)
        VALUES (@PriorityName, @Sequence);

        SELECT SCOPE_IDENTITY() AS PriorityId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
