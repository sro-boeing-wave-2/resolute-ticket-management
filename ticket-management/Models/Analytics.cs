using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class Analytics
    {
        int id;
        DateTime date;
        int customerid;
        double csatscore;
        string avgresolutiontime;

        [Key]
        public int Id { get => id; set => id = value; }
        public DateTime Date { get => date; set => date = value; }
        public string Avgresolutiontime { get => avgresolutiontime; set => avgresolutiontime = value; }
        public double Csatscore { get => csatscore; set => csatscore = value; }
        public int Customerid { get => customerid; set => customerid = value; }
    }
}
