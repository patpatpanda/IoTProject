using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureFunctionRed.Models;

namespace AzureFunctionRed.Data
{
	public class AzureDbContext : DbContext
	{
		public AzureDbContext(DbContextOptions<AzureDbContext> dbContextOptions) : base(dbContextOptions)
		{

		}
		public DbSet<LampModel> LampModels { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<LampModel>(entity => entity.HasKey(c => c.Id));
		}
	}
}
