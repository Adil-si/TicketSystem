using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSystem.Domain.Models
{
  public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
