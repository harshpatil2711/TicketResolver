/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Generate next ticket number

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketGenerateNumber
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LastNumber VARCHAR(9);
    DECLARE @NextNumber VARCHAR(9);
    DECLARE @NextSeq INT;

    SELECT @LastNumber = TicketNumber
    FROM TicketResolverTicket
    ORDER BY TicketId DESC;

    IF @LastNumber IS NULL
    BEGIN
        SET @NextNumber = 'TKT000001';
    END
    ELSE
    BEGIN
        SET @NextSeq = CAST(SUBSTRING(@LastNumber, 4, 6) AS INT) + 1;
        SET @NextNumber = 'TKT' + RIGHT('000000' + CAST(@NextSeq AS VARCHAR(6)), 6);
    END

    SELECT @NextNumber AS TicketNumber;
END
GO