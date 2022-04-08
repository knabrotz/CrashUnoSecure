using CrashUno.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Components
{
    public class TypesViewComponent : ViewComponent
    {
        private IRepository repo { get; set; }

        public TypesViewComponent (IRepository temp)
        {
            repo = temp;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedType = RouteData?.Values["crashseverityid"];
            
            var types = repo.Crash
                .Select(x => x.crash_severity_id)
                .Distinct()
                .OrderBy(x => x);

            return View(types);
        }
    }
}
