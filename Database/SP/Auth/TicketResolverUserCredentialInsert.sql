
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Insert user credential (password hash)

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverUserCredentialInsert
    @UserId       INT,
    @PasswordHash VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO TicketResolverUserCredential (UserId, PasswordHash)
        VALUES (@UserId, @PasswordHash);

        SELECT SCOPE_IDENTITY() AS CredentialId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
