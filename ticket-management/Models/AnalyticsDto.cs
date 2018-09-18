using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class AnalyticsDto
    {
        int customerid;
        double csatscore;
        string avgresolutiontime;

        public int Customerid { get => customerid; set => customerid = value; }
        public double Csatscore { get => csatscore; set => csatscore = value; }
        public string Avgresolutiontime { get => avgresolutiontime; set => avgresolutiontime = value; }
    }
}
