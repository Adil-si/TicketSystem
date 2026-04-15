using System;

namespace TicketSystem.Domain.Models
{
    public class TicketAssignee
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

     
        public Ticket? Ticket { get; set; }
        public ApplicationUser? User { get; set; }
    }
}