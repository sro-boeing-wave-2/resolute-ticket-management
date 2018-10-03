#region MS Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
#endregion
#region Third Party Libraries
using Newtonsoft.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using RabbitMQ.Client;
#endregion
#region Custom Directives
using ticket_management.contract;
using ticket_management.Models;
using ticket_management.Utilities;
#endregion
namespace ticket_management.Services
{
    public class TicketService : ITicketService
    {
        private readonly TicketContext _context;

        public TicketService(IOptions<Settings> settings)
        {
            _context = new TicketContext(settings);
            GetAgents().Wait();
            GetEndUsers().Wait();

        }
     
        /// <summary>
        /// Retrieves all the Tickets present
        /// </summary>
        /// <returns>List of Tickets
        /// </returns>
        public IEnumerable<Ticket> GetTickets()
        {
            return _context.TicketCollection.AsQueryable().ToList();
        }

        /// <summary>
        /// To retrieve a Ticket by the Id of the Ticket
        /// </summary>
        /// <param name="id">The Id of the Ticket required</param>
        /// <returns>Ticket</returns>
        public async Task<Ticket> GetById(string id)
        {

            var filter = Builders<Ticket>.Filter.Eq("TicketId", id);
            var ticket = await _context.TicketCollection.Find(filter).FirstOrDefaultAsync();
            return ticket;

        }

        /// <summary>
        /// Returns all the count of all the open,closed,due and total tickets 
        /// </summary>
        /// <param name="agentemailid">The email Id of an agent</param>
        /// <returns>TicketCount Dto which contains counts of different categories </returns>
        public async Task<TicketCount> GetCount(string agentemailid)
        {
            var filter = Builders<Ticket>.Filter;
            var ticket = new TicketCount
            {
                Open = _context.TicketCollection.Find(x => x.AgentEmailid == agentemailid && x.Status == "open").ToList().Count(),
                Closed = _context.TicketCollection.Find(x => x.Status == "close" && x.AgentEmailid == agentemailid).ToList().Count(),
                Due = _context.TicketCollection.Find(filter.Eq("Status", "due")).ToList().Count(),
                Total = await _context.TicketCollection.CountDocumentsAsync(new BsonDocument())
            };
            return (ticket);
        }

        /// <summary>
        /// Assigns a ticket to an agent using the RoundRobin Algorithm
        /// </summary>
        /// <param name="id">The Id of the ticket to which agent needs to be assigned</param>
        /// <returns>EmailId of an Agent</returns>
        public async Task<string> AssignEmail(string id)
        {            
            string agentEmailId = "";
            var filterEmail = Builders<Ticket>.Filter;
            try
            {
                var agentfilter = Builders<Agents>.Filter;
                long ticketCount = await _context.TicketCollection.CountDocumentsAsync(new BsonDocument()) - await _context.TicketCollection.Find(filterEmail.Eq("AgentEmailid", "bot")).CountDocumentsAsync();
                long agentCount = _context.AgentsCollection.CountDocuments(new BsonDocument());
                Console.WriteLine(ticketCount);
                Console.WriteLine(agentCount);
                long agentId = (ticketCount + 1) % agentCount;
                agentEmailId = _context.AgentsCollection.Find(agentfilter.Eq("Id", agentId)).ToList()[0].Email;
            }
            catch (Exception e){
                Console.WriteLine(e);
                agentEmailId = "null";
            }
            var filter = Builders<Ticket>.Filter;
            var update = Builders<Ticket>.Update;
            await _context.TicketCollection.UpdateOneAsync(filter.Eq("TicketId", id),update.Set(x => x.AgentEmailid , agentEmailId));
            return agentEmailId;
        }

        /// <summary>
        /// Creates a Ticket with the given Query and the User EmailID
        /// </summary>
        /// <param name="chat">consists of the Query and the User EmailId</param>
        /// <returns>A created Ticket</returns>
        public async Task<Ticket> CreateTicket(ChatDto chat)
        {
            Ticket ticket = new Ticket
            {
                TicketId = ObjectId.GenerateNewId().ToString(),

                AgentEmailid = "bot",
                Closedon = null,
                CreatedOn = DateTime.Now,
                Description = chat.Query,
                Intent = null,
                Feedbackscore = null,
                Priority = "Low",
                Status = "open",
                UpdatedBy = null,
                UpdatedOn = null,
                UserEmailId = chat.UserEmail
            };

            var filter = Builders<EndUser>.Filter.Eq("Email", chat.UserEmail);
            ticket.UserName = (await _context.EndUsersCollection.Find(filter).FirstOrDefaultAsync()).Name;
            ticket.UserImageUrl = (await _context.EndUsersCollection.Find(filter).FirstOrDefaultAsync()).ProfileImgUrl;
            var filterTicket = Builders<Ticket>.Filter.Eq("UserEmailId", chat.UserEmail);
            await _context.TicketCollection.InsertOneAsync(ticket);

            var factory = new ConnectionFactory() { HostName = Environment.GetEnvironmentVariable("MACHINE_LOCAL_IPV4")};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var model = connection.CreateModel();
                var properties = model.CreateBasicProperties();
                properties.Persistent = true;
                String jsonified = JsonConvert.SerializeObject(await _context.TicketCollection.Find(filterTicket).FirstOrDefaultAsync());
                var body = Encoding.UTF8.GetBytes(jsonified);
                model.BasicPublish("ticket-notification", "ticket-notification", properties, body);
                Console.WriteLine("Message Sent");
            }


            return (ticket);

        }


        /// <summary>
        /// Returns the Count of tickets resolved, Average Resolution Time and CSAT score
        /// </summary>
        /// <param name="agentemail">The email of the agent who has logged in</param>
        /// <returns>A Dto containing the Analytics Data</returns>
        public AnalyticsUIDto GetAnalytics()

        {
            AnalyticsUIDto Analyticsdata = new AnalyticsUIDto
            {
                Analyticscsat = new List<AnalyticsCsatDto>()
            };

            Analyticsdata.Analyticscsat.AddRange(
                _context.AnalyticsCollection.AsQueryable().Where(x => x.Csatscore >= 0 ).Select(
                    x => new AnalyticsCsatDto { Date = x.Date, Csatscore = x.Csatscore }
                    )
                );

            TicketCount count = new TicketCount();
            int closedTickets =  _context.TicketCollection.AsQueryable().Where(x => x.Status == "close").ToList().Count;
            int openTickets = _context.TicketCollection.AsQueryable().Where(x => x.Status == "open").ToList().Count;
            int dueTickets = _context.TicketCollection.AsQueryable().Where(x => x.Status == "due").ToList().Count;
            Analyticsdata.Analyticscount = new List<AnalyticsCountDto>()
            {
               
                    new AnalyticsCountDto
                    {
                        Count = closedTickets,
                        Tickettype = "Closed"
                    },
                    new AnalyticsCountDto
                    {
                        Count = dueTickets,
                        Tickettype = "Due"
                    },
                    new AnalyticsCountDto
                    {
                        Count = openTickets,
                        Tickettype = "Open"
                    }
                
            };
            DateTime date = DateTime.Now;
            List<Analytics> avgResolutionTime = _context.AnalyticsCollection.AsQueryable().ToList();
            Analyticsdata.Avgresolutiontime = new List<AvgResolutionTime>();
            foreach (Analytics i in avgResolutionTime)
            {
                if (i.Date.Date == date.AddDays(-1).Date)
                { 
                    Analyticsdata.Avgresolutiontime.AddRange(i.Avgresolutiontime);
                }
            }
            //Analyticsdata.Avgresolutiontime = _context.AnalyticsCollection.AsQueryable().Where(x => x.Date.Date == date.Date).Select(x => x.Avgresolutiontime).ToList()[0];
            return Analyticsdata;
        }
        
        /// <summary>
        /// To edit the ticket based on the parameters passed
        /// </summary>
        /// <param name="ticketid">The Id of the Ticket</param>
        /// <param name="status">The Status of the Ticket</param>
        /// <param name="priority">The Priority if the ticket</param>
        /// <param name="intent">The intent of the Query submitted by the User</param>
        /// <param name="feedbackscore">The feedback score provided by the User</param>
        /// <param name="agentemailid">The email of the agent the ticket has been assigned to</param>
        public async Task EditTicket(string ticketid, string status, string priority , string intent, int feedbackscore, string agentemailid)
        {

            var sid = new ObjectId(ticketid);
            var filter = Builders<Ticket>.Filter.Eq("TicketId", sid);
            var ticket =  _context.TicketCollection.Find(filter).FirstOrDefault();

            var update = Builders<Ticket>.Update
                        .Set(x => x.Status, status ?? ticket.Status)
                        .Set(x => x.Priority, priority ?? ticket.Priority)
                        .Set(x => x.Intent, intent ?? ticket.Intent)
                        .Set(x => x.Feedbackscore, (feedbackscore == 0) ? ticket.Feedbackscore : feedbackscore)
                        .Set(x => x.Closedon , (status == "close") ? DateTime.Now : ticket.Closedon)
                        .Set(x => x.UpdatedOn, DateTime.Now)
                        .Set(x=> x.UpdatedBy , ticket.AgentEmailid);           
            
            _context.TicketCollection.UpdateOne(filter, update);
            var filterTicket = Builders<Ticket>.Filter.Eq("AgentEmailId", ticket.AgentEmailid);
            var factory = new ConnectionFactory() { HostName = Environment.GetEnvironmentVariable("MACHINE_LOCAL_IPV4")};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var model = connection.CreateModel();
                var properties = model.CreateBasicProperties();
                properties.Persistent = true;
                String jsonified = JsonConvert.SerializeObject(await _context.TicketCollection.Find(filterTicket).FirstOrDefaultAsync());
                var body = Encoding.UTF8.GetBytes(jsonified);
                model.BasicPublish("ticket-notification", "tasks", properties, body);
                Console.WriteLine("Message Sent");
            }

        }

        public string updatefeedbackScore(string id, feedback data)
        {
            var filter = Builders<Ticket>.Filter.Eq("TicketId", id);
            var update = Builders<Ticket>.Update
                        .Set(x => x.Feedbackscore, data.feedbackScore);
            _context.TicketCollection.UpdateOne(filter, update);

            return "feedbackUpdated";
        }


        /// <summary>
        /// Returns the Paged List Of Tickets Based on the filter Parameters provided
        /// </summary>
        /// <param name="agentemailid">The Email of the Agent</param>
        /// <param name="useremailid">The Email of the End User</param>
        /// <param name="priority">The Priority if the ticket</param>
        /// <param name="status">The Status of the Ticket</param>
        /// <param name="pageno">The Page Number</param>
        /// <param name="size">The number of Tickets to be shown on each page</param>
        /// <returns>List of tickets and the paging parameters</returns>
        public PagedList<Ticket> GetTickets(string agentemailid, string useremailid , string priority, string status, int pageno, int size)
        {
            pageno = (pageno == 0) ? 1 : pageno;
            size = (size == 0) ? 10 : size;

            //needs to be removed later
            if (status != "open" && status != "close")
                agentemailid = null;

            return new PagedList<Ticket>(_context.TicketCollection.AsQueryable().Where(x =>
            (string.IsNullOrEmpty(status) || x.Status == status) &&
            (string.IsNullOrEmpty(priority) || x.Priority == priority) &&
            (string.IsNullOrEmpty(useremailid) || x.UserEmailId == useremailid) &&
            (string.IsNullOrEmpty(agentemailid) || x.AgentEmailid == agentemailid)
            ).OrderByDescending(x=>x.CreatedOn).ToList(), pageno, size);

            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Analytics> PushAnalytics()
        {
            DateTime date = DateTime.Now;
            List<Ticket> ClosedTickets = _context.TicketCollection.AsQueryable()
                .Where(x => x.Status == "close").ToList();

            int ticketscore = 0;

            int totalticketcount = 0;

            foreach (Ticket Cticket in ClosedTickets)
            {
                    if (Cticket.Feedbackscore > 0)
                    {
                        totalticketcount++;
                        if (Cticket.Feedbackscore > 3)
                            ticketscore += Cticket.Feedbackscore.Value;
                    }
            }
            double csatscore;
            try
            {
                csatscore = (double)ticketscore / totalticketcount;
            }
            catch
            {
                csatscore = 0;
            }

            //HttpClient http = new HttpClient();
            //string url =  Constants.BASE_URL + Constants.GET_INTENT;
            //HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            //requestMessage.Headers.Add("Access", "Allow_Service");
            //var response = await http.SendAsync(requestMessage);
            //var result = await response.Content.ReadAsStringAsync();
            //Intent intents = JsonConvert.DeserializeObject<Intent>(result);
            //List<Ticket> listOfTickets = new List<Ticket>();
            List<AvgResolutionTime> avgResolutionTime = new  List<AvgResolutionTime>();
            //if (intents.results.Count() != 0)
            //{
            //    foreach (string intent in intents.results)
            //    {
            //        TimeSpan totalhours = new TimeSpan();
            //        AvgResolutionTime avgresolutiondata = new AvgResolutionTime();
            //        listOfTickets = _context.TicketCollection.AsQueryable().Where(x => x.Status == "close" && x.Intent == intent).ToList();
            //        avgresolutiondata.Intent = intent;
            //        foreach (Ticket ticket in listOfTickets)
            //        {
            //            totalhours += (DateTime)ticket.Closedon - (DateTime)ticket.CreatedOn;
            //        }
            //        avgresolutiondata.Avgresolutiontime = totalhours.Hours;
            //        avgResolutionTime.Add(avgresolutiondata);
            //    }
            //}


            AvgResolutionTime avgResolutiontempdata = new AvgResolutionTime() {
                Avgresolutiontime = 00, Intent = "noData"
                };
                avgResolutionTime.Add(avgResolutiontempdata);
            Analytics scheduledData = new Analytics
            {
                Date = date.Date,
                Avgresolutiontime = avgResolutionTime,
                Csatscore = csatscore
            };
            await _context.AnalyticsCollection.InsertOneAsync(scheduledData);
            return scheduledData;
        }
        /// <summary>
        /// Returns a sorted list of agents who have resolved the most number of tickets
        /// </summary>
        /// <returns>List of Agents Dtos</returns>
        public List<TopAgentsDto> GetTopAgents()
        {
            var listOfAgents = _context.TicketCollection.AsQueryable().Where(x => x.Status == "close" && x.AgentEmailid != "bot")
              .GroupBy(x => x.AgentEmailid).OrderByDescending(x => x.Count());
            List<TopAgentsDto> agentsList = new List<TopAgentsDto>();
            foreach (var agentTickets in listOfAgents)
            {
                var agent = _context.AgentsCollection.AsQueryable().Where(x => x.Email == agentTickets.Key).ToList()[0];

                TopAgentsDto agentDto = new TopAgentsDto
                {
                    NumberOfTicketsResolved = agentTickets.Count(),
                    Name = agent.Name,
                    ProfileImageUrl = agent.ProfileImgUrl
                };
                agentsList.Add(agentDto);
            }
            return agentsList;
        }
        /// <summary>
        /// Gets all the Agents of an Organisation from OnBoarding Module
        /// </summary>
        /// <returns></returns>
        public async Task GetAgents()
        {
            HttpClient httpclient = new HttpClient();
            string url = Constants.BASE_URL + Constants.GET_AGENTS;
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("Access", "Allow_Service");
            var response = await httpclient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            Agents[] responsejson = JsonConvert
                    .DeserializeObject<Agents[]>(result);
            await _context.AgentsCollection.InsertManyAsync(responsejson);

        }
        /// <summary>
        /// Gets all the End Users of an Organisation from OnBoarding Module
        /// </summary>
        /// <returns></returns>
        public async Task GetEndUsers()
        {
            HttpClient httpclient = new HttpClient();
            string url = Constants.BASE_URL + Constants.GET_USERS;
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("Access", "Allow_Service");
            var response = await httpclient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            EndUser[] responsejson = JsonConvert
                    .DeserializeObject<EndUser[]>(result);
            await _context.EndUsersCollection.InsertManyAsync(responsejson);
        }

    }

}

