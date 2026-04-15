using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSystem.Domain.Models
{
    public class BlockedBy
    {
        public int Id { get; set; }
        public int TicketId { get; set; }

        public Ticket? Ticket { get; set; }

        // T block
        public int BlockedByTicketId { get; set; }
        public Ticket? BlockedByTicket { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
