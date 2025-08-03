using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace PartnerTransactionAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var entryAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var logRepository = LogManager.GetRepository(entryAssembly);
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
           
            builder.Services.AddControllers();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
