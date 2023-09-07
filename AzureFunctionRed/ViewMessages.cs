using System;
using System.Text;
using Azure.Messaging.EventHubs;
using AzureFunctionRed.Data;
using AzureFunctionRed.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionRed
{
    public class ViewMessages
    {
	    private readonly AzureDbContext _db;
		private readonly ILogger<ViewMessages> _logger;
        
        public ViewMessages(ILogger<ViewMessages> logger,AzureDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [Function(nameof(ViewMessages))]
		public void Run([EventHubTrigger("iothub-ehub-iot-warrio-25230142-3ad8e367d4", Connection = "IotHubEndpoint")] EventData[] events)
		{
			foreach (EventData @event in events)
			{
				var data = Encoding.UTF8.GetString(@event.Body.ToArray());
				_logger.LogInformation("Event Body: {body}", data);

				
			}
		}

	}
}
