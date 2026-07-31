
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Insert new ticket

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketInsert
    @TicketNumber VARCHAR(9),
    @Subject      NVARCHAR(200),
    @Description  NVARCHAR(MAX),
    @CategoryId   INT,
    @PriorityId   INT,
    @CreatedBy    INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO TicketResolverTicket 
            (TicketNumber, Subject, Description, CategoryId, PriorityId, StatusId, CreatedBy)
        VALUES 
            (@TicketNumber, @Subject, @Description, @CategoryId, @PriorityId, 1, @CreatedBy);

        DECLARE @TicketId INT = SCOPE_IDENTITY();

        INSERT INTO TicketResolverTicketStatusHistory 
            (TicketId, OldStatusId, NewStatusId, PreviousAssignedTo, CurrentAssignedTo, ChangeReason, CreatedBy)
        VALUES 
            (@TicketId, NULL, 1, NULL, NULL, 'Ticket created', @CreatedBy);

        SELECT @TicketId AS TicketId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
