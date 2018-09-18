using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class AnalyticsUIDto
    {
        List<AnalyticsCountDto> analyticscount;
        List<AnalyticsCsatDto> analyticscsat;
        string avgresolutiontime;

        public List<AnalyticsCountDto> Analyticscount { get => analyticscount; set => analyticscount = value; }       
        public string Avgresolutiontime { get => avgresolutiontime; set => avgresolutiontime = value; }
        public List<AnalyticsCsatDto> Analyticscsat { get => analyticscsat; set => analyticscsat = value; }
    }
    public class AnalyticsCsatDto
    {
        DateTime date;
        double csatscore;

        public DateTime Date { get => date; set => date = value; }
        public double Csatscore { get => csatscore; set => csatscore = value; }
    }
    public class AnalyticsCountDto
    {
        string tickettype;
        long count;

        public string Tickettype { get => tickettype; set => tickettype = value; }
        public long Count { get => count; set => count = value; }
    }
}
