using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;

namespace TicketSystem.Domain.Models
{
    public class Ticket
    {

        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty ;
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ? UpdatedAt {  get; set; } 
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<BlockedBy> BlockedByTickets { get; set; } = new List<BlockedBy>();
        public ICollection<BlockedBy> BlocksTickets { get; set; } = new List<BlockedBy>();
        public ICollection<TicketAssignee> Assignees { get; set; } = new List<TicketAssignee>();
        public TicketPriority Priority { get; set; } = TicketPriority.Medium; //neu
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
    public enum TicketStatus
    {
        Open,
        InProgress,
        Closed
    }
    public enum TicketPriority
    {
        Low,      
        Medium,   
        High,     
        Urgent    
    }
}
