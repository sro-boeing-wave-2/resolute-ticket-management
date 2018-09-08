using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ticket_management.Models
{
    public enum Status
{
    open, close, due
}
    public class Ticket
{
    long ticketId;
    string subject;
    string description;
    long agentid;
    long departmentid;
    string source;
    string priority;
    Status status;
    long sla;
    List<Comments> comment;
    DateTime createdOn;
    String createdBy;
    DateTime updatedOn;
    long userid;
    long customerid;
    string updatedBy;
    string connectionid;
    [Key]
    public long TicketId { get => ticketId; set => ticketId = value; }
    public string Subject { get => subject; set => subject = value; }
    public string Description { get => description; set => description = value; }
    public string Source { get => source; set => source = value; }
    public string Priority { get => priority; set => priority = value; }
    public Status Status { get => status; set => status = value; }
    public long Sla { get => sla; set => sla = value; }
    public List<Comments> Comment { get => comment; set => comment = value; }
    public DateTime CreatedOn { get => createdOn; set => createdOn = value; }
    public string CreatedBy { get => createdBy; set => createdBy = value; }
    public DateTime UpdatedOn { get => updatedOn; set => updatedOn = value; }
    public string UpdatedBy { get => updatedBy; set => updatedBy = value; }
    public long Agentid { get => agentid; set => agentid = value; }
    public long Departmentid { get => departmentid; set => departmentid = value; }
    public long Userid { get => userid; set => userid = value; }
    public long Customerid { get => customerid; set => customerid = value; }
    public string Connectionid { get => connectionid; set => connectionid = value; }
    }

}
