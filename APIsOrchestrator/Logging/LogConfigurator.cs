using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using Serilog.Exceptions;
using Serilog.Templates;
using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]
namespace Logging
{
    public static class LogConfigurator
    {
        public static void Configure()
        {

            var log = new LoggerConfiguration()
               .Enrich.WithExceptionDetails()
               .MinimumLevel.Information()
               .WriteTo.Console(new ExpressionTemplate("{ {@timestamp: @t, level: @l, message: @m, ..@p, elapsed: undefined(), class: undefined()} }\n"))
               .WriteTo.File(new ExpressionTemplate("{ {@timestamp: @t, level: @l, message: @m, ..@p, elapsed: undefined(), class: undefined()} }\n"), "logs/application_log.txt", rollingInterval: RollingInterval.Day)
               .CreateLogger();

            Log.Logger = log;

            Log.Information("Log configured");
        }
    }
}
