
-- ============================================================
-- SECTION: Auth
-- ============================================================

/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get latest unverified OTP by email and purpose

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverOtpGetLatest
    @Email   VARCHAR(200),
    @Purpose VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 
        OtpId,
        UserId,
        Email,
        OtpCode,
        Purpose,
        ExpiryDate,
        IsVerified,
        CreatedDate
    FROM TicketResolverOtpVerification
    WHERE Email = @Email
        AND Purpose = @Purpose
        AND IsVerified = 0
    ORDER BY CreatedDate DESC;
END
