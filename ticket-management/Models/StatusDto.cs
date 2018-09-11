using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class StatusDto
    {
        int ticketid;
        string status;

        public int Ticketid { get => ticketid; set => ticketid = value; }
        public string Status { get => status; set => status = value; }
    }
}
