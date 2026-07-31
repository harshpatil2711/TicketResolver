
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Deactivate all refresh tokens for a user

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverRefreshTokenDeactivateAll
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverRefreshToken
        SET IsActive = 0,
            ModifiedDate = GETDATE()
        WHERE UserId = @UserId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
