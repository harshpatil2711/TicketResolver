/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Update ticket status with history

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketUpdateStatus
    @TicketId    INT,
    @NewStatusId INT,
    @ModifiedBy  INT,
    @ChangeReason NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @OldStatusId INT;

        SELECT @OldStatusId = StatusId
        FROM TicketResolverTicket
        WHERE TicketId = @TicketId AND IsActive = 1;

        UPDATE TicketResolverTicket
        SET StatusId = @NewStatusId,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = GETDATE(),
            ResolvedDate = CASE WHEN @NewStatusId = 5 THEN GETDATE() ELSE ResolvedDate END,
            ClosedDate = CASE WHEN @NewStatusId = 6 THEN GETDATE() ELSE ClosedDate END
        WHERE TicketId = @TicketId AND IsActive = 1;

        INSERT INTO TicketResolverTicketStatusHistory 
            (TicketId, OldStatusId, NewStatusId, PreviousAssignedTo, CurrentAssignedTo, ChangeReason, CreatedBy)
        VALUES 
            (@TicketId, @OldStatusId, @NewStatusId, @ModifiedBy, @ModifiedBy, @ChangeReason, @ModifiedBy);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO