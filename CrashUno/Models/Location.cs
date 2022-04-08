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
        [Required]
        public int loc_id { get; set; }
        public string city { get; set; }
    }
}
