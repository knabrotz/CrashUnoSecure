using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Models.ViewModels
{
    public class LocationViewModel
    {
        public IQueryable<Location> Location { get; set; }
        public LocationPageInfo LocationPageInfo { get; set; }
    }
}
