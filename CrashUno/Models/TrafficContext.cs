using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Models
{
    public class TrafficContext : DbContext
    {
        public TrafficContext(DbContextOptions<TrafficContext> options) : base(options)
        {

        }

        public DbSet<Crash> Crash { get; set; }
        public DbSet<Location> Location { get; set; }
    }
}
