using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.contract
{
    public interface ISourceDto
    {
        string Customerhandle { get; set; }
        string Description { get; set; }
        string Sourceid { get; set; }
        string Userhandle { get; set; }
    }
}
