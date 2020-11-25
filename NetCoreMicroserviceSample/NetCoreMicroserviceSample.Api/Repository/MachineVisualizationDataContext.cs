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

        public DbSet<MachineSetting> MachineSettings { get; set; }

        public DbSet<MachineSwitch> MachineSwitches { get; set; }

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
                SvgImage = string.Empty
            });
            modelBuilder.Entity<Machine>().HasData(new
            {
                Id = Guid.NewGuid(),
                Name = "Machine 2",
                Description = "Description 2",
                SvgImage = string.Empty
            });
            modelBuilder.Entity<Machine>().HasData(new
            {
                Id = Guid.NewGuid(),
                Name = "Machine 3",
                Description = "Description 3",
                SvgImage = string.Empty
            });

            modelBuilder.Entity<MachineSetting>().HasData(new
            {
                Id = new Guid("c0927560-36b9-403b-b9cf-d5a96d7cc075"),
                MachineId = machineGuid1,
                Name = "Speed",
                Description = "Speed of crane hook",
                Value = 1d,
                PositionX = 124,
                PositionY = -7
            });

            modelBuilder.Entity<MachineSetting>().HasData(new
            {
                Id = Guid.NewGuid(),
                MachineId = machineGuid1,
                Name = "Setting 2",
                Description = "Description Setting 2",
                Value = 132.54,
                PositionX = 439,
                PositionY = -7
            });

            modelBuilder.Entity<MachineSwitch>().HasData(new
            {
                Id = new Guid("87a217b2-90e7-48a7-a477-7b2d2bf40f49"),
                MachineId = machineGuid1,
                Name = "StopStartHook",
                Description = "Start/Stop hook",
                Value = true,
                PositionX = 212,
                PositionY = 80
            });

            modelBuilder.Entity<MachineSwitch>().HasData(new
            {
                Id = Guid.NewGuid(),
                MachineId = machineGuid1,
                Name = "Switch 2",
                Description = "Switch Setting 2",
                Value = false,
                PositionX = 139,
                PositionY = 282
            });
        }
    }
}
