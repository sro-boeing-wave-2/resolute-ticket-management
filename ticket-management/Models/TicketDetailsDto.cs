﻿using System;using System.Collections.Generic;using System.Linq;using System.Threading.Tasks;namespace ticket_management.Models{    public class TicketDetailsDto    {        string name;        long id;        Status status;        List<Comments> comment;        string priority;        string subject;        string description;        long userid;        string connectionid;        public string Name { get => name; set => name = value; }        public long Id { get => id; set => id = value; }        public Status Status { get => status; set => status = value; }        public List<Comments> Comment { get => comment; set => comment = value; }        public string Priority { get => priority; set => priority = value; }        public string Subject { get => subject; set => subject = value; }        public string Description { get => description; set => description = value; }
        public long Userid { get => userid; set => userid = value; }
        public string Connectionid { get => connectionid; set => connectionid = value; }
    }}