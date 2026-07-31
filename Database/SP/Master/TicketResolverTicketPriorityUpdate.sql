
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Update priority

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketPriorityUpdate
    @PriorityId   INT,
    @PriorityName NVARCHAR(50),
    @Sequence     INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM TicketResolverTicketPriority WHERE PriorityName = @PriorityName AND PriorityId != @PriorityId AND IsActive = 1)
        BEGIN
            RAISERROR('Priority name already exists.', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM TicketResolverTicketPriority WHERE Sequence = @Sequence AND PriorityId != @PriorityId AND IsActive = 1)
        BEGIN
            RAISERROR('Priority sequence already exists.', 16, 1);
            RETURN;
        END

        UPDATE TicketResolverTicketPriority
        SET PriorityName = @PriorityName,
            Sequence = @Sequence,
            ModifiedDate = GETDATE()
        WHERE PriorityId = @PriorityId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
