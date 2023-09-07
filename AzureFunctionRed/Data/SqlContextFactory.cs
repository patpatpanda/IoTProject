using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionRed.Data
{
	public class SqlContextFactory : IDesignTimeDbContextFactory<AzureDbContext>
	{
		private readonly IConfiguration _configuration;

		public SqlContextFactory(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public SqlContextFactory()
		{

		}

		public AzureDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<AzureDbContext>();
			optionsBuilder.UseSqlServer(
				"Server=tcp:azuremrred.database.windows.net,1433;Initial Catalog=AzureLamp;Persist Security Info=False;User ID=MrAdmin;Password={Hejsan123!};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

			return new AzureDbContext(optionsBuilder.Options);
		}
	}
}
