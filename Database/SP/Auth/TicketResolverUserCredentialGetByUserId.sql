/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get credential by user ID

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverUserCredentialGetByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CredentialId,
        UserId,
        PasswordHash
    FROM TicketResolverUserCredential
    WHERE UserId = @UserId AND IsActive = 1;
END
GO