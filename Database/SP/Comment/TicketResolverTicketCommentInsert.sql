
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Insert new comment

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketCommentInsert
    @TicketId       INT,
    @UserId         INT,
    @CommentText    NVARCHAR(MAX),
    @IsInternalNote BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO TicketResolverTicketComment 
            (TicketId, UserId, CommentText, IsInternalNote)
        VALUES 
            (@TicketId, @UserId, @CommentText, @IsInternalNote);

        SELECT SCOPE_IDENTITY() AS CommentId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
