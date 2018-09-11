using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class CommentDto
    {
        long ticketid;
        string comment;
        long createdby;
        public long Ticketid { get => ticketid; set => ticketid = value; }
        public string Comment { get => comment; set => comment = value; }
        public long Createdby { get => createdby; set => createdby = value; }
    }
}
