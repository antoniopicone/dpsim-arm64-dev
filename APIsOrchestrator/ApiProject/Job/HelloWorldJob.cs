using System;
using Quartz;
using Serilog;
using System.Threading.Tasks;

namespace ApiSupport.Job	
{
    [DisallowConcurrentExecution]
    public class HelloWorldJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Log.Information("Eseguo JOB Schedulato");
            return Task.CompletedTask;
        }
    }
}
