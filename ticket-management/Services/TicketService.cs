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

        public IEnumerable<Ticket> GetTickets(int departmentid)
        {
            return _context.Ticket.Include(x => x.Comment).Where(x => (x.Departmentid == departmentid)).ToList();
        }

        public async Task<TicketDetailsDto> GetById(int id)
        {
            Ticket CompleteTicketDetails = await _context.Ticket
                                                .Include(x => x.Comment)
                                                .SingleOrDefaultAsync(x => x.TicketId == id);
            //HttpClient httpclient = new HttpClient();
            //string url = "http://172.23.238.225:5002/api/endusers/query?id=" + CompleteTicketDetails.Userid;
            //var response = await httpclient.GetAsync(url);
            //var userName = await response.Content.ReadAsStringAsync();
            TicketDetailsDto Ticket = new TicketDetailsDto();
            Ticket.Id = CompleteTicketDetails.TicketId;
            Ticket.Name = "userName";
            Ticket.Priority = CompleteTicketDetails.Priority;
            Ticket.Status = CompleteTicketDetails.Status;
            Ticket.Subject = CompleteTicketDetails.Subject;
            Ticket.Description = CompleteTicketDetails.Description;
            Ticket.Comment = CompleteTicketDetails.Comment;
            return Ticket;
        }

        public async Task<TicketCount> GetCount(int agentId, int departmentid)
        {
            return new TicketCount
            {
                Open = await _context.Ticket.Where(x => (x.Status == Status.open&&x.Agentid==agentId&&x.Departmentid== departmentid)).CountAsync(),
                Closed = await _context.Ticket.Where(x => (x.Status == Status.close && x.Agentid == agentId && x.Departmentid == departmentid)).CountAsync(),
                Due = await _context.Ticket.Where(x => (x.Status == Status.due && x.Departmentid == departmentid)).CountAsync(),
                Total = await _context.Ticket.Where(x=> (x.Departmentid == departmentid)).CountAsync()
            };
        }

        public IEnumerable<Ticket> GetByStatus(string status, int agentId, int departmentid)
        {
            return _context.Ticket.Include(x => x.Comment)
                .Where(ticket => ticket.Status.ToString() == status
                              && ticket.Departmentid == departmentid
                              && ticket.Agentid == ((ticket.Status == Status.open || ticket.Status == Status.close) ? agentId: ticket.Agentid)
                );
        }

        public async Task<Ticket> CreateTicket(ChatDto chat)
        {
            //HttpClient httpclient = new HttpClient();
            //string url = "http://172.23.238.225:5002/api/endusers/query?email=" + chat.Customerhandle;
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
            ticket.CreatedBy = 1;
            ticket.CreatedOn = DateTime.Now;
            ticket.UpdatedBy = 1;
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

        public async Task EditTicketByStatus(StatusDto ticket) {
            Ticket Originalticket = await _context.Ticket.SingleOrDefaultAsync(x=> x.TicketId == ticket.Ticketid);
            Originalticket.Status = ticket.Status == "close" ? Status.close : ticket.Status == "open" ? Status.open : Status.due;
            await _context.SaveChangesAsync();
        }

        public async Task EditTicketByPriority(PriorityDto ticket)
        {
            Ticket editticket = await _context.Ticket.SingleOrDefaultAsync(x => x.TicketId == ticket.Ticketid);
            editticket.Priority = ticket.Priority;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTicketComment(CommentDto comment)
        {
            Ticket editticket = await _context.Ticket.Include(x => x.Comment).SingleOrDefaultAsync(x => x.TicketId == comment.Ticketid);
            Comments commentToBeAdded = new Comments();
            commentToBeAdded.Comment = comment.Comment;
            commentToBeAdded.CreatedBy = comment.Createdby;
            commentToBeAdded.UpdatedBy = comment.Createdby;
            commentToBeAdded.UpdatedOn = DateTime.Now;
            commentToBeAdded.CreatedOn = DateTime.Now;
            editticket.Comment.Add(commentToBeAdded);
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
           )
           );
         }
    }

}
