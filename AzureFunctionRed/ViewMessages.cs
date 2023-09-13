using System;
using System.Text;
using System.Text.Json;
using Azure.Messaging.EventHubs;
using AzureFunctionRed.Data;
using AzureFunctionRed.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonException = Newtonsoft.Json.JsonException;


namespace AzureFunctionRed
{
	public class ViewMessages
	{
		private readonly AzureDbContext _db;
		private readonly ILogger<ViewMessages> _logger;

		public ViewMessages(ILogger<ViewMessages> logger, AzureDbContext db)
		{
			_logger = logger;
			_db = db;
		}


		[Function(nameof(ViewMessages))]
		public void Run(
			[EventHubTrigger("iothub-ehub-iot-warrio-25230142-3ad8e367d4", Connection = "IotHubEndpoint")]
			EventData[] events)
		{
			foreach (EventData @event in events)
			{
				var data = Encoding.UTF8.GetString(@event.Body.ToArray());

				
				var parsedData = ParseEventData(data);
				_logger.LogInformation("Event Body: {body}", data);

				parsedData = new ParseData()
				{
					Id = parsedData.Id,
					Date = parsedData.Date,
					DeviceMessage = parsedData.DeviceMessage

				};
				int lol = 1;
				SaveDataToLocalDatabase(parsedData);
			}
		}

		private void SaveDataToLocalDatabase(ParseData parsedData)
		{
			
				
				_db.ParseDatas.Add(parsedData);
				_db.SaveChanges();
			
				
			
		}

		public ParseData ParseEventData(string eventData)
		{
			try
			{
				var parsedData = JsonConvert.DeserializeObject<ParseData>(eventData);

				
				return parsedData;
			}
			catch (JsonException ex)
			{
				
				_logger.LogError("Error parsing JSON data: {ex.Message}");

				
				return null;
			}
		}

		
	}



}
	
