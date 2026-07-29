/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Insert OTP for verification

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverOtpInsert
    @UserId    INT = NULL,
    @Email     VARCHAR(200),
    @OtpCode   VARCHAR(10),
    @Purpose   VARCHAR(20),
    @ExpiryDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO TicketResolverOtpVerification 
            (UserId, Email, OtpCode, Purpose, ExpiryDate, IsVerified)
        VALUES 
            (@UserId, @Email, @OtpCode, @Purpose, @ExpiryDate, 0);

        SELECT SCOPE_IDENTITY() AS OtpId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO