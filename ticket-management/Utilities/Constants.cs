using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Utilities
{
    public class Constants
    {
        public static string BASE_URL = "http://" + Environment.GetEnvironmentVariable("MACHINE_LOCAL_IPV4");
        public static string GET_INTENT = "/intent/getIntent";
        public static string GET_AGENTS = "/agents";
        public static string GET_USERS = "/endusers";
        
    }
}
