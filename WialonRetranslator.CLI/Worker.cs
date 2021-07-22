using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WialonRetranslator.Core.Interfaces;

namespace WialonRetranslator.CLI
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IProtocolParserService _protocolParserService;

        public Worker(ILogger<Worker> logger, IProtocolParserService protocolParserService)
        {
            _logger = logger;
            _protocolParserService = protocolParserService;
            protocolParserService.StartTCPServer();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}