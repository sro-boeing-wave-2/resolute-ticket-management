using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ticket_management.Models;

namespace ticket_management.Models
{
    public class TicketContext : DbContext
    {
        public TicketContext (DbContextOptions<TicketContext> options)
            : base(options)
        {
        }

        public DbSet<ticket_management.Models.Ticket> Ticket { get; set; }

        public DbSet<ticket_management.Models.Analytics> Analytics { get; set; }
    }
}
