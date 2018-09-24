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
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> GetAllTicket([FromHeader] long departmentid)
        {
            return new string[] {"ticket-management-value-1", "ticket-management-value-2"};
        }
    }
}