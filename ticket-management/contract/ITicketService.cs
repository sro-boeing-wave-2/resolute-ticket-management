#region MS Directives
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion
#region Custom Directives
using ticket_management.Models;
#endregion

namespace ticket_management.contract
{
    public interface ITicketService
    {
        Task<Ticket> CreateTicket(ChatDto chat);
        Task EditTicket(string ticketid, string status, string priority, string intent, int feedbackscore, string agentemailid);
        AnalyticsUIDto GetAnalytics();
        Task<Ticket> GetById(string id);
        Task<TicketCount> GetCount(string agentEmailId);
        PagedList<Ticket> GetTickets(string agentEmailId, string userEmailId, string priority, string status, int pageno, int size);
        List<TopAgentsDto> GetTopAgents();
        Task<Analytics> PushAnalytics();
        Task GetAgents();
        string updatefeedbackScore(string id, feedback data);
        Task<string> AssignEmail(string id);
        IEnumerable<Ticket> GetTickets();
        Task<List<Analytics>> pushData(List<Analytics> data);
        //List<Ticket> GetRecentTickets(string agentEmailId);
    }
}
