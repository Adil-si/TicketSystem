using System;

namespace TicketSystem.Domain.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
                
        public int TicketId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
                
        public Ticket? Ticket { get; set; }
        public ApplicationUser? Author { get; set; }
    }
}