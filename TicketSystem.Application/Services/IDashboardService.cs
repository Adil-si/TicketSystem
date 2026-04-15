using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Application.DTOs;

namespace TicketSystem.Application.Services
{
   public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync();

    }
}
