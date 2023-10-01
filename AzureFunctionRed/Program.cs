using AzureFunctionRed.Data;
using AzureFunctionRed.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureAppConfiguration(config => config.AddJsonFile("local.settings.json"))
	.ConfigureServices((builder, services) =>
	{
		services.AddDbContext<AzureDbContext>(options =>
		{
			options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
		});

		// Hämta ServiceProvider för att få tillgång till DbContext senare
		var serviceProvider = services.BuildServiceProvider();
		var dbContext = serviceProvider.GetRequiredService<AzureDbContext>();

		// Skapa databasen om den inte redan existerar
		dbContext.Database.EnsureCreated();
	})
	.Build();

host.Run();