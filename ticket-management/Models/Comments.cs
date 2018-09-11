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
    long createdBy;
    DateTime updatedOn;
    long updatedBy;
    [Key]
    public long CommentId { get => id; set => id = value; }
    public string Comment { get => comment; set => comment = value; }
    public DateTime CreatedOn { get => createdOn; set => createdOn = value; }
    public long CreatedBy { get => createdBy; set => createdBy = value; }
    public DateTime UpdatedOn { get => updatedOn; set => updatedOn = value; }
    public long UpdatedBy { get => updatedBy; set => updatedBy = value; }
    }
}
