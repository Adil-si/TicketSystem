using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSystem.Domain.Models
{
    public class Attachment
    {                            //telecharger de Ticket
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; } 
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int TicketId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;

       
        public Ticket? Ticket { get; set; }
        public ApplicationUser? Uploader { get; set; }
    }
}
