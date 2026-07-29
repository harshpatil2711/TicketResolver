/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Insert new user

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverUserInsert
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

        IF EXISTS (SELECT 1 FROM TicketResolverUser WHERE Email = @Email)
        BEGIN
            RAISERROR('Email already registered.', 16, 1);
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM TicketResolverUser WHERE Mobile = @Mobile)
        BEGIN
            RAISERROR('Mobile number already registered.', 16, 1);
            RETURN;
        END

        INSERT INTO TicketResolverUser (RoleId, FirstName, LastName, Email, Mobile)
        VALUES (@RoleId, @FirstName, @LastName, @Email, @Mobile);

        SELECT SCOPE_IDENTITY() AS UserId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO