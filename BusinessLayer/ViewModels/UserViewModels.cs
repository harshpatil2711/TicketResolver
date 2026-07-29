using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicketResolver.Models;

namespace TicketResolver.ViewModels
{
    public class UserSearchViewModel
    {
        public string SearchTerm { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public List<UserListItemViewModel> Results { get; set; }
        public List<TicketRole> Roles { get; set; }
    }

    public class UserListItemViewModel
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string FullName => FirstName + " " + LastName;
    }

    public class UserCreateViewModel
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Mobile { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public List<TicketRole> Roles { get; set; }
    }

    public class UserEditViewModel
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Mobile { get; set; }

        [Required]
        public int RoleId { get; set; }

        public bool IsActive { get; set; }
        public List<TicketRole> Roles { get; set; }
    }
}
