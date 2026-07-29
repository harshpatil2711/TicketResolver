/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get all active users of a specific role

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverUserGetByRoleId
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.UserId,
        u.FirstName,
        u.LastName,
        u.Email,
        u.Mobile
    FROM TicketResolverUser u
    WHERE u.RoleId = @RoleId AND u.IsActive = 1
    ORDER BY u.FirstName, u.LastName;
END
GO