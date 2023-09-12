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

				// Parse the data as needed
				var parsedData = ParseEventData(data);
				_logger.LogInformation("Event Body: {body}", data);
				// Save the parsed data to the local database
				//SaveDataToLocalDatabase(parsedData);
			}
		}

		private void SaveDataToLocalDatabase(ParseData parsedData)
		{
			
				// Use Entity Framework or your preferred database access method to save data
				_db.ParseDatas.Add(parsedData);
				_db.SaveChanges();
			
				
			
		}

		public ParseData ParseEventData(string eventData)
		{
			try
			{
				// Assuming eventData is a JSON string, you can use Newtonsoft.Json to deserialize it
				var parsedData = JsonConvert.DeserializeObject<ParseData>(eventData);

				// If the structure of eventData matches the ParsedData class, you can directly deserialize it
				return parsedData;
			}
			catch (JsonException ex)
			{
				// Handle JSON parsing errors here, e.g., log the error
				_logger.LogError("Error parsing JSON data: {ex.Message}");

				// Return null or an appropriate default value in case of parsing failure
				return null;
			}
		}

		
	}



}
	
