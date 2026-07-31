
/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Soft delete category

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketCategoryDelete
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverTicketCategory
        SET IsActive = 0,
            ModifiedDate = GETDATE()
        WHERE CategoryId = @CategoryId AND IsActive = 1;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
