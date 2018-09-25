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

        public IEnumerable<Ticket> GetTickets(long departmentid)
        {
            return _context.Ticket.Include(x => x.Comment).Where(x => (x.Departmentid == departmentid)).ToList();
        }

       

        public async Task<AnalyticsUIDto> GetAnalytics(long agentId, long departmentid)
        {
            AnalyticsUIDto Analyticsdata = new AnalyticsUIDto();
            Analyticsdata.Analyticscsat = new List<AnalyticsCsatDto>();
            Analyticsdata.Analyticscsat.AddRange(
                _context.Analytics.ToList().Select(
                    x => new AnalyticsCsatDto { Date = x.Date, Csatscore = x.Csatscore }
                    )
                );
            
            TicketCount count = new TicketCount();
            Analyticsdata.Analyticscount = new List<AnalyticsCountDto>();
            Analyticsdata.Analyticscount.AddRange(
                new List<AnalyticsCountDto> {
                    new AnalyticsCountDto
                    {
                        Count = await _context.Ticket.Where(x => (x.Status == Status.close && x.Agentid == agentId && x.Departmentid == departmentid)).CountAsync(),
                        Tickettype = "Closed"
                    },
                    new AnalyticsCountDto
                    {
                        Count = await _context.Ticket.Where(x => (x.Status == Status.due && x.Departmentid == departmentid)).CountAsync(),
                        Tickettype = "Due"
                    },
                    new AnalyticsCountDto
                    {
                        Count = await _context.Ticket.Where(x => (x.Status == Status.open && x.Agentid == agentId && x.Departmentid == departmentid)).CountAsync(),
                        Tickettype = "Open"
                    }
                }
            );
            Analyticsdata.Avgresolutiontime = "5:04:23";
            return Analyticsdata;
        }

        public async Task<List<TopAgentsDto>> GetTopAgents()
        {
            
            var listOfAgents = _context.Ticket.Where(x => x.Status == Status.close)
                .GroupBy(x => x.Agentid).OrderByDescending(x => x.Count()).Take(3).ToList();
            List<TopAgentsDto> agentsList = new List<TopAgentsDto>(); foreach (var agentTickets in listOfAgents)
            {
                HttpClient httpclient = new HttpClient();

                string url = "http://35.221.125.153/agents/leaderboard?id=" + +agentTickets.Key; 

                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                requestMessage.Headers.Add("Access", "Allow_Service");

                var response = await httpclient.SendAsync(requestMessage);
                
                var result = await response.Content.ReadAsStringAsync();
                TopAgentsDto responsejson = JsonConvert
                    .DeserializeObject<TopAgentsDto>(result);
                TopAgentsDto agent = new TopAgentsDto
                {
                    NumberOfTicketsResolved = agentTickets.Count(),
                    Name = responsejson.Name,
                    DepartmentName = responsejson.DepartmentName,
                    ProfileImageUrl = responsejson.ProfileImageUrl
                };
                agentsList.Add(agent);
            }
            return agentsList;
        }


        public async Task<TicketDetailsDto> GetById(long id, string email, string name)
        {
            Ticket CompleteTicketDetails = await _context.Ticket
                                                .Include(x => x.Comment)
                                                .SingleOrDefaultAsync(x => x.TicketId == id);
            Console.WriteLine("Passed ticket fetch");
            Console.WriteLine(CompleteTicketDetails.Agentid);
            HttpClient httpclient = new HttpClient();
            string url = "http://35.221.125.153/endusers/" + CompleteTicketDetails.Userid;
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("Access", "Allow_Service");
            var response = await httpclient.SendAsync(requestMessage);            
            var result = await response.Content.ReadAsStringAsync();            
            Console.WriteLine("Passed call from api");
            OnboardingUser.EndUser responsejson = JsonConvert.DeserializeObject<OnboardingUser.EndUser>(result);
            Console.WriteLine("deserialoze");
            var userName = await response.Content.ReadAsStringAsync();
            TicketDetailsDto Ticket = new TicketDetailsDto();
            Ticket.Id = CompleteTicketDetails.TicketId;
            Ticket.Name = responsejson.Name;
            Ticket.Userid = CompleteTicketDetails.Userid;
            Ticket.Priority = CompleteTicketDetails.Priority;
            Ticket.Status = CompleteTicketDetails.Status;
            Ticket.Subject = CompleteTicketDetails.Subject;
            Ticket.Description = CompleteTicketDetails.Description;
            Ticket.Comment = CompleteTicketDetails.Comment;
            Ticket.Connectionid = CompleteTicketDetails.Connectionid;
            Ticket.Email = email;
            Ticket.Agentname = name;
            return Ticket;
        }

        public async Task<TicketCount> GetCount(long agentId, long departmentid)
        {
            return new TicketCount
            {
                Open = await _context.Ticket.Where(x => (x.Status == Status.open && x.Agentid == agentId && x.Departmentid == departmentid)).CountAsync(),
                Closed = await _context.Ticket.Where(x => (x.Status == Status.close && x.Agentid == agentId && x.Departmentid == departmentid)).CountAsync(),
                Due = await _context.Ticket.Where(x => (x.Status == Status.due && x.Departmentid == departmentid)).CountAsync(),
                Total = await _context.Ticket.Where(x => (x.Departmentid == departmentid)).CountAsync()
            };
        }

        public IEnumerable<Ticket> GetByStatus(string status, long agentId, long departmentid)
        {
            return _context.Ticket.Include(x => x.Comment)
                .Where(ticket => ticket.Status.ToString() == status
                              && ticket.Departmentid == departmentid
                              && ticket.Agentid == ((ticket.Status == Status.open || ticket.Status == Status.close) ? agentId : ticket.Agentid)
                );
        }

        public async Task<Ticket> CreateTicket(ChatDto chat)
        {
            //HttpClient httpclient = new HttpClient();
            //string url = "http://35.221.125.153/endusers/query?email=" + chat.Customerhandle;
            //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            //requestMessage.Headers.Add("Access", "Allow_Service");
            //var response = await httpclient.SendAsync(requestMessage);
            //var result = await response.Content.ReadAsStringAsync();
            //OnboardingUser.User responsejson = JsonConvert.DeserializeObject<OnboardingUser.User>(result);

            Ticket ticket = new Ticket();
            ticket.Description = chat.Description;
            ticket.Source = "twitter";
            ticket.Sla = 123;
            ticket.Priority = "High";
            ticket.Status = Status.open;
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
            Random random = new Random();
            ticket.Feedbackscore =  random.Next(0,5);
            
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
        //Update analytics csat score
        public async Task<Analytics> UpdateAnalytics() {
            DateTime date = DateTime.Now;
            List<int> ticketscore = new List<int>();
            ticketscore = _context.Ticket.Where(x => x.UpdatedOn.Date == date.Date && x.Status == Status.close && x.Feedbackscore > 3).Select(x => x.Feedbackscore).ToList();
            List<int> totalticketscore = new List<int>();
            totalticketscore = _context.Ticket.Where(x => x.UpdatedOn.Date == date.Date && x.Status == Status.close && x.Feedbackscore > 0).Select(x => x.Feedbackscore).ToList();
            Console.WriteLine((double)ticketscore.Sum() / totalticketscore.Count());
            Console.WriteLine(ticketscore.Sum());
            Console.WriteLine(totalticketscore.Count());
            double csatscore = (double)ticketscore.Sum() / totalticketscore.Count();
            Analytics scheduledData = new Analytics();
            scheduledData.Date = date.Date;
            scheduledData.Customerid = '1';
            scheduledData.Avgresolutiontime = "5:0:0";
            scheduledData.Csatscore = csatscore;
            await _context.Analytics.AddAsync(scheduledData);
            await _context.SaveChangesAsync();
            return scheduledData;
        }

        

        public IEnumerable<Ticket> Filter(long agentid, long departmentid, long userid, long customerid,
                string source, string priority, string status, int pageno, int size)
        {
            if (pageno == 0 || size == 0) {
                pageno = 1;
                size = 20;
            }
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
           ).Skip((pageno - 1)*size).Take(size); 
         }
    }

}
