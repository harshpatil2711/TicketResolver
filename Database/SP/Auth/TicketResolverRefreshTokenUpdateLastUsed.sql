
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Update last used date for refresh token

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverRefreshTokenUpdateLastUsed
    @RefreshTokenId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverRefreshToken
        SET LastUsedDate = GETDATE(),
            ModifiedDate = GETDATE()
        WHERE RefreshTokenId = @RefreshTokenId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
