using Microsoft.EntityFrameworkCore;
using NetCoreMicroserviceSample.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMicroserviceSample.Api.Repository
{
    public class MachineVisualizationDataContext : DbContext
    {
        public DbSet<Machine> Machines { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MachineVisualizationDataContext(DbContextOptions<MachineVisualizationDataContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add demo data
            var machineGuid1 = Guid.NewGuid();
            modelBuilder.Entity<Machine>().HasData(new
            {
                Id = machineGuid1,
                Name = "Machine 1",
                Description = "Description 1",
                SvgImage = ""
            });
            modelBuilder.Entity<Machine>().HasData(new
            {
                Id = Guid.NewGuid(),
                Name = "Machine 2",
                Description = "Description 2",
                SvgImage = ""
            });
            modelBuilder.Entity<Machine>().HasData(new
            {
                Id = Guid.NewGuid(),
                Name = "Machine 3",
                Description = "Description 3",
                SvgImage = ""
            });
        }
    }
}
