using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class ChatDto
    {
        string connectionid;
        string description;
        long userid;
        string customerhandle;
        public string Description { get => description; set => description = value; }
        public string Customerhandle { get => customerhandle; set => customerhandle = value; }
        public long Userid { get => userid; set => userid = value; }
        public string Connectionid { get => connectionid; set => connectionid = value; }
    }
}
