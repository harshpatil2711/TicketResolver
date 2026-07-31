
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Update last used date and deactivate old token

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverRefreshTokenRotate
    @OldRefreshTokenId INT,
    @NewTokenHash      VARCHAR(500),
    @NewExpiryDate     DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @UserId INT;

        SELECT @UserId = UserId
        FROM TicketResolverRefreshToken
        WHERE RefreshTokenId = @OldRefreshTokenId;

        UPDATE TicketResolverRefreshToken
        SET IsActive = 0,
            ModifiedDate = GETDATE()
        WHERE RefreshTokenId = @OldRefreshTokenId;

        INSERT INTO TicketResolverRefreshToken 
            (UserId, TokenHash, ExpiryDate)
        VALUES 
            (@UserId, @NewTokenHash, @NewExpiryDate);

        SELECT SCOPE_IDENTITY() AS RefreshTokenId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
