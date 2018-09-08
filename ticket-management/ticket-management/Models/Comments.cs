using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ticket_management.Models
{
    
    public class Comments
    {
    long id;
    string comment;
    DateTime createdOn;
    string createdBy;
    DateTime updatedOn;
    string updatedBy;
    [Key]
    public long CommentId { get => id; set => id = value; }
    public string Comment { get => comment; set => comment = value; }
    public DateTime CreatedOn { get => createdOn; set => createdOn = value; }
    public string CreatedBy { get => createdBy; set => createdBy = value; }
    public DateTime UpdatedOn { get => updatedOn; set => updatedOn = value; }
    public string UpdatedBy { get => updatedBy; set => updatedBy = value; }
    }

}
