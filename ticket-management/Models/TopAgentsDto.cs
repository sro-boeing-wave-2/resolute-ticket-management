using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class TopAgentsDto
    {
        string name;
        string profileImageUrl;
        string departmentName;
        int numberOfTicketsResolved; public int NumberOfTicketsResolved { get => numberOfTicketsResolved; set => numberOfTicketsResolved = value; }
        public string Name { get => name; set => name = value; }
        public string ProfileImageUrl { get => profileImageUrl; set => profileImageUrl = value; }
        public string DepartmentName { get => departmentName; set => departmentName = value; }
    }
}
