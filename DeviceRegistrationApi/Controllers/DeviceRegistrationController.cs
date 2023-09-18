using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DeviceRegistrationApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DeviceRegistrationController : ControllerBase
	{
		// Hårdkodade uppgifter för registrerade enheter (ersätt med dina riktiga data)
		private static readonly List<string> RegisteredDevices = new List<string>
		{
			"red",
			
		};

		[HttpGet("{deviceId}")]
		public IActionResult CheckDeviceRegistration(string deviceId)
		{
			try
			{
				// Simulerar en kontroll mot en lista över registrerade enheter
				if (RegisteredDevices.Contains(deviceId))
				{
					return Ok(new { IsRegistered = true });
				}
				else
				{
					return Ok(new { IsRegistered = false });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { ErrorMessage = ex.Message });
			}
		}

		

	}



}
