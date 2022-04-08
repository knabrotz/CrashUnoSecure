using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Models
{
    public class Location
    {
        [Key]
        [Required(ErrorMessage = "Please enter a location")]
        public int loc_id { get; set; }
        [Required(ErrorMessage = "Please Enter a City Name")]

        public string city { get; set; }
    }
}
