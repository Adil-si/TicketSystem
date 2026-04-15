using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSystem.Domain.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }

        public string SenderId { get; set; } = string.Empty;
        public ApplicationUser? Sender { get; set; }

   
        public String ReceiverId { get; set; } = string.Empty;
        public ApplicationUser? Receiver { get; set; }

    }
}
