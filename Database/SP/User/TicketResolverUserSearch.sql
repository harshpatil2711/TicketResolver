/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Search and paginate users

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverUserSearch
    @SearchTerm NVARCHAR(200) = NULL,
    @RoleId     INT = NULL,
    @IsActive   BIT = NULL,
    @PageNumber INT = 1,
    @PageSize   INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Skip INT = (@PageNumber - 1) * @PageSize;

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
    WHERE 
        (@SearchTerm IS NULL OR 
         u.FirstName LIKE '%' + @SearchTerm + '%' OR 
         u.LastName LIKE '%' + @SearchTerm + '%' OR 
         u.Email LIKE '%' + @SearchTerm + '%')
        AND (@RoleId IS NULL OR u.RoleId = @RoleId)
        AND (@IsActive IS NULL OR u.IsActive = @IsActive)
    ORDER BY u.CreatedDate DESC
    OFFSET @Skip ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*) AS TotalCount
    FROM TicketResolverUser u
    WHERE 
        (@SearchTerm IS NULL OR 
         u.FirstName LIKE '%' + @SearchTerm + '%' OR 
         u.LastName LIKE '%' + @SearchTerm + '%' OR 
         u.Email LIKE '%' + @SearchTerm + '%')
        AND (@RoleId IS NULL OR u.RoleId = @RoleId)
        AND (@IsActive IS NULL OR u.IsActive = @IsActive);
END
GO