/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Assign ticket to support executive

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketAssign
    @TicketId    INT,
    @AssignedTo  INT,
    @AssignedBy  INT,
    @ChangeReason NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @OldStatusId INT;
        DECLARE @OldAssignedTo INT;

        SELECT @OldStatusId = StatusId,
               @OldAssignedTo = AssignedTo
        FROM TicketResolverTicket
        WHERE TicketId = @TicketId AND IsActive = 1;

        UPDATE TicketResolverTicket
        SET AssignedTo = @AssignedTo,
            StatusId = 2,
            ModifiedBy = @AssignedBy,
            ModifiedDate = GETDATE()
        WHERE TicketId = @TicketId AND IsActive = 1;

        INSERT INTO TicketResolverTicketStatusHistory 
            (TicketId, OldStatusId, NewStatusId, PreviousAssignedTo, CurrentAssignedTo, ChangeReason, CreatedBy)
        VALUES 
            (@TicketId, @OldStatusId, 2, @OldAssignedTo, @AssignedTo, @ChangeReason, @AssignedBy);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO