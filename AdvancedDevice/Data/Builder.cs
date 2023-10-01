﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedDevice.Data
{
	public class Builder
	{


		public string AppBuilder()
		{
			var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
			var config = builder.Build();

			var connectionString = config.GetConnectionString("DefaultConnection");

			return connectionString;
		}

	}

}
