/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Verify OTP by code and email

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverOtpVerify
    @Email   VARCHAR(200),
    @OtpCode VARCHAR(10),
    @Purpose VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @OtpId INT = NULL;
    DECLARE @UserId INT = NULL;

    SELECT TOP 1 
        @OtpId = OtpId,
        @UserId = UserId
    FROM TicketResolverOtpVerification
    WHERE Email = @Email
        AND OtpCode = @OtpCode
        AND Purpose = @Purpose
        AND IsVerified = 0
        AND ExpiryDate > GETDATE()
    ORDER BY CreatedDate DESC;

    IF @OtpId IS NOT NULL
    BEGIN
        UPDATE TicketResolverOtpVerification
        SET IsVerified = 1
        WHERE OtpId = @OtpId;

        SELECT 
            @OtpId AS OtpId,
            @UserId AS UserId,
            1 AS IsValid;
    END
    ELSE
    BEGIN
        SELECT 
            NULL AS OtpId,
            NULL AS UserId,
            0 AS IsValid;
    END
END
GO