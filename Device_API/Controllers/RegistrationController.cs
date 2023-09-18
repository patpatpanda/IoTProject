using Microsoft.AspNetCore.Mvc;

namespace Device_API.Controllers
{
	[ApiController]
	[Route("api/checkregistration")]
	public class RegistrationController : ControllerBase
	{
		// Simulerad lista över registrerade enheter (byt ut med din faktiska lagring)
		private List<string> registeredDevices = new List<string>
		{
			"LampDevice",

		};

		[HttpGet]
		public IActionResult CheckRegistration()
		{
			try
			{
				

				string deviceId = "red"; 

				bool isRegistered = IsDeviceRegistered(deviceId);

				if (isRegistered)
				{
					return Ok(true); 
				}
				else
				{
					return Ok(false); 
				}
			}
			catch (Exception ex)
			{
				return BadRequest($"Fel vid registreringskontroll: {ex.Message}");
			}
		}

		
		private bool IsDeviceRegistered(string deviceId)
		{
			

			List<string> registeredDevices = new List<string>
			{
				"red",

			};

			return registeredDevices.Contains(deviceId);
		}

	}
}
