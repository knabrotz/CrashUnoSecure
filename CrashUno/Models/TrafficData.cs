using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Models
{
    public class TrafficData
    {
        public float pedestrian_involved { get; set; }
        public float bicyclist_involved { get; set; }
        public float motorcycle_involved { get; set; }
        public float improper_restraint { get; set; }
        public float unrestrained { get; set; }
        public float dui { get; set; }
        public float intersection_related { get; set; }
        public float single_vehicle { get; set; }
        public float distracted_driving { get; set; }
        public float city_w_jordan { get; set; }

        public Tensor<float> AsTensor()
        {
            float[] data = new float[]
            {
                pedestrian_involved, bicyclist_involved, motorcycle_involved,
                improper_restraint, unrestrained, dui, intersection_related,
                single_vehicle, distracted_driving, city_w_jordan
            };

            int[] dimensions = new int[] { 1, 10 };
            return new DenseTensor<float>(data, dimensions);
        }
    }
}
