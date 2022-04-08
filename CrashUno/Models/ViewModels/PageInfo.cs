using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Models.ViewModels
{
    public class PageInfo
    {
        public int TotalNumCrashes {get; set;}
        public int CrashesPerPage { get; set; }
        public int CurrentPage { get; set; }

        //how many pages
        public int TotalPages => (int)Math.Ceiling((double)TotalNumCrashes / CrashesPerPage);
    }
}
