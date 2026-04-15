namespace TicketSystem.Application.DTOs
{
    public class DashboardDto
    {
       
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int InProgressTickets { get; set; }

        // Projekt für später
        public int TotalProjects { get; set; }
        public int OpenProjects { get; set; }
        public int ClosedProjects { get; set; }

        // User Statistiken
        public int TotalUsers { get; set; }
        public int AdminCount { get; set; }
        public int UserCount { get; set; }

        // Tickets 
        public List<TicketDto> RecentTickets { get; set; } = new();
    }

    public class TicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}