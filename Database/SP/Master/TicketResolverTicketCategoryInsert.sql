/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Insert new category

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketCategoryInsert
    @CategoryName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM TicketResolverTicketCategory WHERE CategoryName = @CategoryName AND IsActive = 1)
        BEGIN
            RAISERROR('Category already exists.', 16, 1);
            RETURN;
        END

        INSERT INTO TicketResolverTicketCategory (CategoryName)
        VALUES (@CategoryName);

        SELECT SCOPE_IDENTITY() AS CategoryId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO