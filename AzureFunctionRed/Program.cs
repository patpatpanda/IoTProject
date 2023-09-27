using AzureFunctionRed.Data;
using AzureFunctionRed.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices((builder, services) =>
	{
		// Replace "YourConnectionString" with your actual connection string
		var connectionString = "Server=localhost;Database=Karlsson;TrustServerCertificate=true;Trusted_Connection=True;MultipleActiveResultSets=true";

		services.AddDbContext<AzureDbContext>(options =>
			options.UseSqlServer(connectionString));
	})
	.Build();

host.Run();