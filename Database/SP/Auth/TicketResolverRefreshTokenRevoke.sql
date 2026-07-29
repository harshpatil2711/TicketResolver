/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Revoke a specific refresh token

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverRefreshTokenRevoke
    @RefreshTokenId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverRefreshToken
        SET RevokedDate = GETDATE(),
            IsActive = 0,
            ModifiedDate = GETDATE()
        WHERE RefreshTokenId = @RefreshTokenId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO