using Microsoft.EntityFrameworkCore;
using ticket_management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ticket_management.contract;
using System.Net.Http;
using Newtonsoft.Json;

namespace ticket_management.Services
{
    public class TicketService : ITicketService
    {
        private readonly TicketContext _context;

        public TicketService(TicketContext context)
        {
            _context = context;
            
    }

        public IEnumerable<Ticket> GetTickets()
        {
            return _context.Ticket.Include(x => x.Comment).ToList();
        }

        public async Task<Ticket> GetById(int id)
        {
            return await _context.Ticket
                .Include(x => x.Comment)
                .SingleOrDefaultAsync(x => x.TicketId == id);
        }

        public async Task<TicketCount> GetCount()
        {
            return new TicketCount
            {
                Open = await _context.Ticket.Where(x => (x.Status == Status.open)).CountAsync(),
                Closed = await _context.Ticket.Where(x => (x.Status == Status.close)).CountAsync(),
                Due = await _context.Ticket.Where(x => (x.Status == Status.due)).CountAsync(),
                Total = await _context.Ticket.CountAsync()
            };
        }

        public IEnumerable<Ticket> GetByStatus(string status)
        {
            return _context.Ticket.Include(x => x.Comment).Where(ticket => ticket.Status.ToString() == status);
        }

        public async Task<Ticket> CreateTicket(ChatDto chat)
        {
            //HttpClient httpclient = new HttpClient();
            //string url = "http://172.23.238.225:5002/api/endusers/query?email=" + chat.Userhandle;
            //var response = await httpclient.GetAsync(url);
            //var result = await response.Content.ReadAsStringAsync();
            //OnboardingUser.User responsejson = JsonConvert.DeserializeObject<OnboardingUser.User>(result);
            Ticket ticket = new Ticket();
            ticket.Description = chat.Description;
            ticket.Source = "twitter";
            ticket.Sla = 123;
            ticket.Priority = "High";
            ticket.Status = Status.close;
            ticket.Agentid = 1;
            ticket.Userid = chat.Userid;
            ticket.Customerid = 1;
            ticket.Departmentid = 1;
            ticket.Subject = "Hello";
            ticket.Connectionid = chat.Connectionid;
            ticket.CreatedBy = "srikant";
            ticket.CreatedOn = DateTime.Now;
            ticket.UpdatedBy = "sandeep";
            ticket.UpdatedOn = DateTime.Now;
            
            ticket.Comment = new List<Comments>();

            _context.Ticket.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task EditTicket(Ticket ticket)
        {
            //Ticket EditTicket =  await _context.Ticket.Include(x => x.Conversation).Include(x => x.Comment).SingleOrDefaultAsync(x => x.TicketId == ticket.TicketId);

            _context.Ticket.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Ticket> Filter(int agentid, int departmentid, int userid, int customerid,
                string source, string priority, string status)
        {
            return _context.Ticket.Include(x => x.Comment).Where(n =>
           (
               n.Agentid == ((agentid != 0) ? agentid : n.Agentid) &&
               n.Departmentid == ((departmentid != 0) ? departmentid : n.Departmentid) &&
               n.Userid == ((userid != 0) ? userid : n.Userid) &&
               n.Customerid == ((customerid != 0) ? customerid : n.Customerid) &&
               n.Source == (String.IsNullOrEmpty(source) ? n.Source : source) &&
               n.Priority == (String.IsNullOrEmpty(priority) ? n.Priority : priority) &&
               n.Status.ToString() == (String.IsNullOrEmpty(status) ? n.Status.ToString() : status)
           ));
        }
    }

}
