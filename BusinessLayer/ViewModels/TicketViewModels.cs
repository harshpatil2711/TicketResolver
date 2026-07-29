using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicketResolver.Models;

namespace TicketResolver.ViewModels
{
    public class TicketSearchViewModel
    {
        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public List<TicketListItemViewModel> Results { get; set; }

        public List<TicketCategory> Categories { get; set; }
        public List<TicketPriority> Priorities { get; set; }
        public List<TicketStatus> Statuses { get; set; }
    }

    public class TicketListItemViewModel
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; }
        public string Subject { get; set; }
        public string CategoryName { get; set; }
        public string PriorityName { get; set; }
        public int PrioritySequence { get; set; }
        public string StatusName { get; set; }
        public int StatusId { get; set; }
        public string CreatedByName { get; set; }
        public string AssignedToName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
    }

    public class TicketCreateViewModel
    {
        [Required]
        [MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int PriorityId { get; set; }

        public List<TicketCategory> Categories { get; set; }
        public List<TicketPriority> Priorities { get; set; }
    }

    public class TicketEditViewModel
    {
        public int TicketId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int PriorityId { get; set; }

        public List<TicketCategory> Categories { get; set; }
        public List<TicketPriority> Priorities { get; set; }
    }

    public class TicketDetailViewModel
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int PriorityId { get; set; }
        public string PriorityName { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public bool IsTerminalState { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public int? AssignedTo { get; set; }
        public string AssignedToName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public bool IsActive { get; set; }

        public List<TicketComment> Comments { get; set; }
        public List<TicketAttachment> Attachments { get; set; }
        public List<TicketStatusHistory> History { get; set; }
        public List<TicketStatus> Statuses { get; set; }
        public List<UserListItem> SupportExecutives { get; set; }
    }

    public class TicketAssignViewModel
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; }
        public string Subject { get; set; }

        [Required]
        public int AssignedTo { get; set; }

        public string ChangeReason { get; set; }
        public List<UserListItem> SupportExecutives { get; set; }
    }

    public class UserListItem
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalTickets { get; set; }
        public int NewTickets { get; set; }
        public int AssignedTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int ReopenedTickets { get; set; }
        public List<TicketListItemViewModel> RecentTickets { get; set; }
        public List<TicketListItemViewModel> UnassignedTickets { get; set; }
    }
}
