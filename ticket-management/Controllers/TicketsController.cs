#region MS Directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
#endregion
#region Custom Directives
using ticket_management.Models;
using ticket_management.contract;
#endregion

namespace ticket_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [Route("leaderboard")]
        public IActionResult GetTopAgents()
        {
            var topAgents = _ticketService.GetTopAgents();
            return Ok(topAgents);
        }

        [Route("detail/{id}")]
        public async Task<IActionResult> GetTicketById([FromRoute] string id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ticket ticket = await _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        [Route("count")]
        public async Task<TicketCount> CountTickets([FromHeader(Name = "email")] string agentEmailId)
        {
            return await _ticketService.GetCount(agentEmailId);

        }

        [Route("filter")]
        public IActionResult GetSortedTickets([FromHeader(Name = "email")] string agentEmailId, [FromQuery] string userEmailId,
            [FromQuery] string priority, [FromQuery] string status, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var model = _ticketService.GetTickets(agentEmailId, userEmailId, priority, status, pageNumber, pageSize);
            TicketOutputModel outputModel = new TicketOutputModel
            {
                Pages = model.TotalPages,
                HasNext = model.HasNextPage,
                HasPrevious = model.HasPreviousPage,
                Tickets = model.List
            };

            return Ok(outputModel);
        }

        [HttpGet("Analytics")]
        public AnalyticsUIDto GetAnalytics()
        {
            return _ticketService.GetAnalytics();
        }

        [HttpGet()]
        public IEnumerable<Ticket> Gettickets()
        {
            return _ticketService.GetTickets();
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateTicket([FromBody] ChatDto chat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ticket ticket = await _ticketService.CreateTicket(chat); ;
            return Ok(ticket);
        }

        [HttpGet("assignagent/{id}")]
        public async Task<IActionResult> AssignAgent([FromRoute] string id) {
            string Email = await _ticketService.AssignEmail(id);
            return Ok(Email);
        }
        [HttpGet("analytics/update")]
        public async Task<IActionResult> CreateAnalysis()
        {
            Analytics analyticsdata = await _ticketService.PushAnalytics();
            return Ok(analyticsdata);

        }

        // PUT: api/Tickets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTicket([FromRoute]string id, [FromQuery] string status, [FromQuery] string priority,
            [FromQuery] string intent, [FromQuery] int feedbackscore, [FromQuery] string agentemailid)
        {
            await _ticketService.EditTicket(id, status, priority, intent, feedbackscore, agentemailid);
            return Ok();
        }

        //updateFeedbackScore
        [HttpPut("{id}/feedback")]
        public IActionResult updateFeedback([FromRoute] string id, [FromBody] feedback data)
        {
            string response = _ticketService.updatefeedbackScore(id, data);
            return Ok(response);
        }

        [HttpPost("addAnalytics")]
        public async Task<IActionResult> addAnalytics([FromBody] List<Analytics> data)
        {
            List<Analytics> response =  await _ticketService.pushData(data);
            return Ok(response);
        }
    }
}
