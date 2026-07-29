/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Soft delete / activate / deactivate user

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverUserSetActiveStatus
    @UserId   INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverUser
        SET IsActive = @IsActive,
            ModifiedDate = GETDATE()
        WHERE UserId = @UserId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO