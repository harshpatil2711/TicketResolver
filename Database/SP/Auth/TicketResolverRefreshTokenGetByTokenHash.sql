
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get refresh token by token hash

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverRefreshTokenGetByTokenHash
    @TokenHash VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        rt.RefreshTokenId,
        rt.UserId,
        rt.TokenHash,
        rt.ExpiryDate,
        rt.LastUsedDate,
        rt.RevokedDate,
        rt.IsActive,
        u.Email,
        u.FirstName,
        u.LastName,
        r.RoleName
    FROM TicketResolverRefreshToken rt
    INNER JOIN TicketResolverUser u ON rt.UserId = u.UserId
    INNER JOIN TicketResolverRole r ON u.RoleId = r.RoleId
    WHERE rt.TokenHash = @TokenHash;
END
