using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Quartz;
using ApiSupport.Job;

namespace RockApi
{
    public class Program
    {


        public static void Main(string[] args)
        {
            var aspnetcoreurl = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");

            //CreateWebHostBuilder(args).Build().Run();
            CreateHostBuilderWithConf(args, aspnetcoreurl).Build().Run();

        }

        public static IHostBuilder CreateHostBuilderWithConf(string[] args, string ASPNETCORE_URLS) =>
    Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(

        webBuilder => { webBuilder.UseStartup<Startup>().UseUrls(ASPNETCORE_URLS); }

        ).UseSerilog(Log.Logger)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                // Register the job, loading the schedule from configuration
                q.AddJobAndTrigger<HelloWorldJob>(hostContext.Configuration);
                //q.AddJobAndTrigger<HelloWorldJob>(hostContext.Configuration);
                //q.AddJobAndTrigger<SendEmailJob>(hostContext.Configuration);
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            // ...
        });


      //  public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      //      WebHost.CreateDefaultBuilder(args)
      //          .UseStartup<Startup>();

    }
}
