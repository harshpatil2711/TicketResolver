
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Update user credential password hash

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverUserCredentialUpdatePassword
    @UserId       INT,
    @PasswordHash VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverUserCredential
        SET PasswordHash = @PasswordHash,
            ModifiedDate = GETDATE()
        WHERE UserId = @UserId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
