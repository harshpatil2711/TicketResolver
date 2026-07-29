/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Invalidate previous OTPs for same email and purpose

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverOtpInvalidatePrevious
    @Email   VARCHAR(200),
    @Purpose VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE TicketResolverOtpVerification
        SET IsVerified = 1
        WHERE Email = @Email
            AND Purpose = @Purpose
            AND IsVerified = 0;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO