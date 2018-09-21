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
        Task<TicketDetailsDto> GetById(long id);
        IEnumerable<Ticket> GetByStatus(string status, long agentId, long departmentid);
        Task<TicketCount> GetCount(long agentId, long departmentid);
        IEnumerable<Ticket> GetTickets(long departmentid);
        Task<AnalyticsUIDto> GetAnalytics(long agentid, long departmentid);
        Task<Analytics> UpdateAnalytics();        
        Task EditTicket(Ticket ticket);
        Task EditTicketByStatus(StatusDto ticket);
        Task EditTicketByPriority(PriorityDto ticket);
        Task UpdateTicketComment(CommentDto comment);
        IEnumerable<Ticket> Filter(long agentid, long departmentid, long userid, long customerid,
                string source, string priority, string status, int pageno, int size);
    }
}
