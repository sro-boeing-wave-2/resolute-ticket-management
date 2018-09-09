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
        Task<Ticket> GetById(int id);
        IEnumerable<Ticket> GetByStatus(string status);
        Task<TicketCount> GetCount();
        IEnumerable<Ticket> GetTickets();
        Task EditTicket(Ticket ticket);
        IEnumerable<Ticket> Filter(int agentid, int departmentid, int userid, int customerid,
                string source, string priority, string status);
    }
}
