using System;
using System.Collections.Generic;
using System.Text;

namespace TicketSystem.Domain.Models
{
    public class Category
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
       


        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}
