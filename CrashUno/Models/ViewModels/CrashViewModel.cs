using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Models.ViewModels
{
    public class CrashViewModel
    {
        public IQueryable<Crash> Crash { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
