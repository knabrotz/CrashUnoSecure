using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Models
{
    public class Crash
    {
        [Key]
        [Required]
        public int crash_id { get; set; }
        public string crash_datetime { get; set; }
        public string route {get; set;}
        public float milepoint { get; set; }
        public float lat_utm_y { get; set; }
        public float long_utm_x { get; set; }
        public string main_road_name { get; set; }
        public int crash_severity_id { get; set; }
        public int work_zone_related { get; set; }
        public int pedestrian_involved { get; set; }
        public int bicyclist_involved { get; set; }
        public int motorcycle_involved { get; set; }
        public int improper_restraint { get; set; }
        public int unrestrained { get; set; }
        public int dui { get; set; }
        public int intersection_related { get; set; }
        public int wild_animal_related { get; set; }
        public int domestic_animal_related { get; set; }
        public int overturn_rollover { get; set; }
        public int commercial_motor_veh_involved { get; set; }
        public int teenage_driver_involved { get; set; }
        public int older_driver_involved { get; set; }
        public int night_dark_condition { get; set; }
        public int single_vehicle { get; set; }
        public int distracted_driving { get; set; }
        public int drowsy_driving { get; set; }
        public int roadway_departure { get; set; }

        [Display (Name = "Category")]
        public int loc_id { get; set; }
        [ForeignKey("loc_id")]
        public Location location { get; set; }

    }
}
