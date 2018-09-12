using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class PriorityDto
    {
        int ticketid;
        string priority;
        public int Ticketid { get => ticketid; set => ticketid = value; }
        public string Priority { get => priority; set => priority = value; }
    }
}
