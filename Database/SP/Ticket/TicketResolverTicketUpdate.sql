/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Update ticket details (only when status is New)

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketUpdate
    @TicketId   INT,
    @Subject    NVARCHAR(200),
    @Description NVARCHAR(MAX),
    @CategoryId INT,
    @PriorityId INT,
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverTicket
        SET Subject = @Subject,
            Description = @Description,
            CategoryId = @CategoryId,
            PriorityId = @PriorityId,
            ModifiedBy = @ModifiedBy,
            ModifiedDate = GETDATE()
        WHERE TicketId = @TicketId 
            AND IsActive = 1 
            AND StatusId = 1
            AND CreatedBy = @ModifiedBy;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO