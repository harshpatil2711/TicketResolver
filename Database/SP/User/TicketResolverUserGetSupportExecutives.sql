/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get all active support executives (for dropdown)

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverUserGetSupportExecutives
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.UserId,
        u.FirstName + ' ' + u.LastName AS FullName,
        u.Email
    FROM TicketResolverUser u
    INNER JOIN TicketResolverRole r ON u.RoleId = r.RoleId
    WHERE r.RoleName = 'Support Executive' AND u.IsActive = 1
    ORDER BY u.FirstName, u.LastName;
END
GO