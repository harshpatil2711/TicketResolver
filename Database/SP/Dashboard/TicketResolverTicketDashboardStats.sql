
-- ============================================================
-- SECTION: Dashboard
-- ============================================================

/*
***********************************************************************************************
    Date            Modified By         Purpose of Modification

1   28 Jul 2026    Initial Creation    Get dashboard statistics

***********************************************************************************************
*/

CREATE PROCEDURE TicketResolverTicketDashboardStats
    @UserId INT = NULL,
    @RoleId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalTickets INT = 0;
    DECLARE @NewTickets INT = 0;
    DECLARE @AssignedTickets INT = 0;
    DECLARE @InProgressTickets INT = 0;
    DECLARE @ResolvedTickets INT = 0;
    DECLARE @ClosedTickets INT = 0;
    DECLARE @ReopenedTickets INT = 0;

    IF @RoleId = 3
    BEGIN
        SELECT @TotalTickets = COUNT(*) FROM TicketResolverTicket WHERE CreatedBy = @UserId AND IsActive = 1;
        SELECT @NewTickets = COUNT(*) FROM TicketResolverTicket WHERE CreatedBy = @UserId AND IsActive = 1 AND StatusId = 1;
        SELECT @AssignedTickets = COUNT(*) FROM TicketResolverTicket WHERE CreatedBy = @UserId AND IsActive = 1 AND StatusId = 2;
        SELECT @InProgressTickets = COUNT(*) FROM TicketResolverTicket WHERE CreatedBy = @UserId AND IsActive = 1 AND StatusId = 3;
        SELECT @ResolvedTickets = COUNT(*) FROM TicketResolverTicket WHERE CreatedBy = @UserId AND IsActive = 1 AND StatusId = 5;
        SELECT @ClosedTickets = COUNT(*) FROM TicketResolverTicket WHERE CreatedBy = @UserId AND IsActive = 1 AND StatusId = 6;
        SELECT @ReopenedTickets = COUNT(*) FROM TicketResolverTicket WHERE CreatedBy = @UserId AND IsActive = 1 AND StatusId = 7;
    END
    ELSE IF @RoleId = 2
    BEGIN
        SELECT @TotalTickets = COUNT(*) FROM TicketResolverTicket WHERE AssignedTo = @UserId AND IsActive = 1;
        SELECT @NewTickets = COUNT(*) FROM TicketResolverTicket WHERE AssignedTo = @UserId AND IsActive = 1 AND StatusId = 1;
        SELECT @AssignedTickets = COUNT(*) FROM TicketResolverTicket WHERE AssignedTo = @UserId AND IsActive = 1 AND StatusId = 2;
        SELECT @InProgressTickets = COUNT(*) FROM TicketResolverTicket WHERE AssignedTo = @UserId AND IsActive = 1 AND StatusId = 3;
        SELECT @ResolvedTickets = COUNT(*) FROM TicketResolverTicket WHERE AssignedTo = @UserId AND IsActive = 1 AND StatusId = 5;
        SELECT @ClosedTickets = COUNT(*) FROM TicketResolverTicket WHERE AssignedTo = @UserId AND IsActive = 1 AND StatusId = 6;
        SELECT @ReopenedTickets = COUNT(*) FROM TicketResolverTicket WHERE AssignedTo = @UserId AND IsActive = 1 AND StatusId = 7;
    END
    ELSE
    BEGIN
        SELECT @TotalTickets = COUNT(*) FROM TicketResolverTicket WHERE IsActive = 1;
        SELECT @NewTickets = COUNT(*) FROM TicketResolverTicket WHERE IsActive = 1 AND StatusId = 1;
        SELECT @AssignedTickets = COUNT(*) FROM TicketResolverTicket WHERE IsActive = 1 AND StatusId = 2;
        SELECT @InProgressTickets = COUNT(*) FROM TicketResolverTicket WHERE IsActive = 1 AND StatusId = 3;
        SELECT @ResolvedTickets = COUNT(*) FROM TicketResolverTicket WHERE IsActive = 1 AND StatusId = 5;
        SELECT @ClosedTickets = COUNT(*) FROM TicketResolverTicket WHERE IsActive = 1 AND StatusId = 6;
        SELECT @ReopenedTickets = COUNT(*) FROM TicketResolverTicket WHERE IsActive = 1 AND StatusId = 7;
    END

    SELECT 
        @TotalTickets AS TotalTickets,
        @NewTickets AS NewTickets,
        @AssignedTickets AS AssignedTickets,
        @InProgressTickets AS InProgressTickets,
        @ResolvedTickets AS ResolvedTickets,
        @ClosedTickets AS ClosedTickets,
        @ReopenedTickets AS ReopenedTickets;
END
