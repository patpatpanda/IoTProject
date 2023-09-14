using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureFunctionRed.Models;
using Microsoft.Extensions.Configuration;

namespace AzureFunctionRed.Data
{
    public class AzureDbContext : DbContext
	{
		private readonly IConfiguration _configuration;

		public AzureDbContext(DbContextOptions<AzureDbContext> dbContextOptions) : base(dbContextOptions)
		{

		}
		
		public DbSet<ParseData> ParseDatas { get; set; }
		
		public DbSet<DeviceStatus> DeviceStatusSet { get; set; }
		public DbSet<DeviceAction> DeviceActions{ get; set; }




	}
}
