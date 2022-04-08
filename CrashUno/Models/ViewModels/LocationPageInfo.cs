using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Models.ViewModels
{
    public class LocationPageInfo
    {
        public int TotalNumLocations { get; set; }
        public int LocationsPerPage { get; set; }
        public int CurrentPage { get; set; }

        //how many pages
        public int TotalPages => (int)Math.Ceiling((double)TotalNumLocations / LocationsPerPage);

    }
}
