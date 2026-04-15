using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;


namespace TicketSystem.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public bool IsLocked { get; set; } = false;
        public DateTime? LockedAt { get; set; }
        public string? LockReason { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<TicketAssignee> AssignedTickets { get; set; } = new List<TicketAssignee>();

    }
}
