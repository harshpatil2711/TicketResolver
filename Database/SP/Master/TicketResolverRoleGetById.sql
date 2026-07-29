/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get role by ID

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverRoleGetById
    @RoleId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        RoleId,
        RoleName
    FROM TicketResolverRole
    WHERE RoleId = @RoleId AND IsActive = 1;
END
GO