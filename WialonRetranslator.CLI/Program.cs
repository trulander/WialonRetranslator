using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WialonRetranslator.BusinessLogic.Services;
using WialonRetranslator.Core.Interfaces;
using WialonRetranslator.DataAccess;
using WialonRetranslator.DataAccess.Repository;

namespace WialonRetranslator.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    
                    services.AddDbContext<WialonDbContext>(options =>
                    {
                        options.UseSqlite(hostContext.Configuration.GetConnectionString("DefaultConnection"));
                    },ServiceLifetime.Singleton);
                    
                    services.AddSingleton<IPointRepository, PointRepository>();
                    services.AddSingleton<IProtocolParserService, ProtocolParserService>();
                    
                    services.AddHostedService<Worker>();
                });
    }
}