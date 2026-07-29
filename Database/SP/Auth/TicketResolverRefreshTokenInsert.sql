/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Insert refresh token

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverRefreshTokenInsert
    @UserId     INT,
    @TokenHash  VARCHAR(500),
    @ExpiryDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO TicketResolverRefreshToken 
            (UserId, TokenHash, ExpiryDate)
        VALUES 
            (@UserId, @TokenHash, @ExpiryDate);

        SELECT SCOPE_IDENTITY() AS RefreshTokenId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO