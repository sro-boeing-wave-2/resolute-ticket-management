using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticket_management.Models;
using ticket_management.contract;
using static ticket_management.Models.OnboardingUser;

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

        // GET: api/Tickets
        [HttpGet]
        public IEnumerable<Ticket> GetAllTicket([FromHeader] long departmentid)
        {
            return _ticketService.GetTickets(departmentid);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetTopAgents()
        {
            var topAgents = await _ticketService.GetTopAgents();
            return Ok(topAgents);
        }

        [HttpGet("Analytics")]
        public async Task<AnalyticsUIDto> GetAnalytics([FromHeader] long agentid, [FromHeader] long departmentid)
        {
            return await _ticketService.GetAnalytics(agentid, departmentid);
        }
        

        [Route("detail/{id}")]
        public async Task<IActionResult> GetTicketById([FromRoute] int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TicketDetailsDto ticket = await _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        //for user
        [Route("count")]
        public async Task<TicketCount> CountTickets([FromHeader] int agentId, [FromHeader] int departmentid)
        {
            return await _ticketService.GetCount(agentId, departmentid);
        }



        [Route("status/{status}")]
        public IEnumerable<Ticket> GetTicketByStatus([FromRoute] string status, [FromHeader] int agentId, [FromHeader] int departmentid)
        {
            return _ticketService.GetByStatus(status, agentId, departmentid);
        }

        [Route("filter")]
        public IEnumerable<Ticket> filterTickets([FromQuery] int agentid,
            [FromQuery] int departmentid,
            [FromQuery] int userid,
            [FromQuery] int customerid,
            [FromQuery] string source,
            [FromQuery] string priority,
            [FromQuery] string status,
            [FromQuery] int page,
            [FromQuery] int pagesize)
        {
            return _ticketService.Filter(agentid, departmentid, userid, customerid,
               source, priority, status, page, pagesize);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] ChatDto chat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ticket ticket = await _ticketService.CreateTicket(chat);
            return Ok(ticket);
        }
     

        [HttpGet("analytics/update")]
        public async Task<IActionResult> createAnalysis1()
        {
            Analytics analyticsdata = await _ticketService.UpdateAnalytics();
            return Ok(analyticsdata);

        }

        [HttpPut("updatecomment")]
        public async Task<IActionResult> UpdateTicketComment([FromBody] CommentDto comment)
        {
            await _ticketService.UpdateTicketComment(comment);
            return Ok(comment);
        }
        // PUT: api/Tickets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTicket([FromRoute] int id, [FromBody] Ticket ticket)
        {
            await _ticketService.EditTicket(ticket);
            return Ok(ticket);
        }

        [HttpPut("status")]
        public async Task<IActionResult> EditTicketByStatus([FromBody] StatusDto ticket) {
            await _ticketService.EditTicketByStatus(ticket);
            return Ok(ticket);
        }
        [HttpPut("priority")]
        public async Task<IActionResult> EditTicketByPriority([FromBody] PriorityDto priority)
        {
            await _ticketService.EditTicketByPriority(priority);
            return Ok(priority);
        }

        IList<Ticket> GetPage(IList<Ticket> list, int page, int pageSize)
        {
            return list.Skip(page * pageSize).Take(pageSize).ToList();
        }
    }
}