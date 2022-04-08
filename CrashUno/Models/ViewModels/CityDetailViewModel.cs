using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Models.ViewModels
{
    public class CityDetailViewModel
    {
        public Location Location { get; set; }
        public Prediction Score { get; set; }
        public Prediction Pedestrian_Prediction { get; set; }
        public Prediction Bicyclist_Prediction { get; set; }
        public Prediction Motorcycle_Prediction { get; set; }
        public Prediction Unrestrained_Prediction { get; set; }
        public Prediction DUI_Prediction { get; set; }
        public Prediction Intersection_Prediction { get; set; }
        public Prediction Distracted_Prediction { get; set; }
        public Prediction SingleVehicle_Prediction { get; set; }
    }
}
