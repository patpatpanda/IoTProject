using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedDevice.Services;

namespace AdvancedDevice.Data
{

	public class AppDbContext : DbContext
	{
		
		public DbSet<LampService> LampServices { get; set; }

		public AppDbContext()
		{

		}

		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer(@"Server=.;Database=IoTDb;Trusted_Connection=True;TrustServerCertificate=true;");
			}
		}
	}
}

