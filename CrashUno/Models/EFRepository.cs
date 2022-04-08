using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Models
{
    public class EFRepository : IRepository
    {
        private TrafficContext context { get; set; }
        public EFRepository (TrafficContext temp)
        {
            context = temp;
        }
        public IQueryable<Crash> Crash => context.Crash;

        public IQueryable<Location> Location => context.Location;
        public void SaveCrashRecord(Crash c)
        {
            context.Update(c);
            context.SaveChanges();
        }

        public void CreateCrashRecord(Crash c)
        {
            context.Add(c);
            context.SaveChanges();
        }

        public void DeleteCrashRecord(Crash c)
        {
            context.Remove(c);
            context.SaveChanges();
        }
    }
}
