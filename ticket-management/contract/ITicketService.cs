using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ticket_management.Models;

namespace ticket_management.contract
{
    public interface ITicketService
    {
        Task<Ticket> CreateTicket(ChatDto chat);
        Task<TicketDetailsDto> GetById(int id);
        IEnumerable<Ticket> GetByStatus(string status, int agentId, int departmentid);
        Task<TicketCount> GetCount(int agentId, int departmentid);
        IEnumerable<Ticket> GetTickets(int departmentid);
        AnalyticsUIDto GetAnalytics();
        Task<Analytics> UpdateAnalytics();        
        Task EditTicket(Ticket ticket);
        Task EditTicketByStatus(StatusDto ticket);
        Task EditTicketByPriority(PriorityDto ticket);
        Task UpdateTicketComment(CommentDto comment);
        IEnumerable<Ticket> Filter(int agentid, int departmentid, int userid, int customerid,
                string source, string priority, string status, int pageno, int size);
    }
}
