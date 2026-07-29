/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Update category

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketCategoryUpdate
    @CategoryId   INT,
    @CategoryName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM TicketResolverTicketCategory WHERE CategoryName = @CategoryName AND CategoryId != @CategoryId AND IsActive = 1)
        BEGIN
            RAISERROR('Category name already exists.', 16, 1);
            RETURN;
        END

        UPDATE TicketResolverTicketCategory
        SET CategoryName = @CategoryName,
            ModifiedDate = GETDATE()
        WHERE CategoryId = @CategoryId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO