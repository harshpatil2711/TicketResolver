/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get all active roles

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverRoleGetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        RoleId,
        RoleName
    FROM TicketResolverRole
    WHERE IsActive = 1
    ORDER BY RoleName;
END
GO