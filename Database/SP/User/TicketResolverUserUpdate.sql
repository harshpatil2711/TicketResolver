/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Update user details

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverUserUpdate
    @UserId    INT,
    @RoleId    INT,
    @FirstName NVARCHAR(100),
    @LastName  NVARCHAR(100),
    @Email     VARCHAR(200),
    @Mobile    VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM TicketResolverUser WHERE Email = @Email AND UserId != @UserId)
        BEGIN
            RAISERROR('Email already registered.', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM TicketResolverUser WHERE Mobile = @Mobile AND UserId != @UserId)
        BEGIN
            RAISERROR('Mobile number already registered.', 16, 1);
            RETURN;
        END

        UPDATE TicketResolverUser
        SET RoleId = @RoleId,
            FirstName = @FirstName,
            LastName = @LastName,
            Email = @Email,
            Mobile = @Mobile,
            ModifiedDate = GETDATE()
        WHERE UserId = @UserId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO