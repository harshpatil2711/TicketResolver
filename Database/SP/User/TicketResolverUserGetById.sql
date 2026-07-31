
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get user by ID with role name

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverUserGetById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.UserId,
        u.RoleId,
        r.RoleName,
        u.FirstName,
        u.LastName,
        u.Email,
        u.Mobile,
        u.CreatedDate,
        u.IsActive
    FROM TicketResolverUser u
    INNER JOIN TicketResolverRole r ON u.RoleId = r.RoleId
    WHERE u.UserId = @UserId;
END
