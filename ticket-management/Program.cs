using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using DotNetEnv;
using System;

namespace ticket_management
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load("./machine_config/.env");
            Console.WriteLine("System NAT Address - ");
            Console.WriteLine(Environment.GetEnvironmentVariable("MACHINE_LOCAL_IPV4"));
            CreateWebHostBuilder(args).Build().Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
