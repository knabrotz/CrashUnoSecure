using CrashUno.Models;
using CrashUno.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno.Controllers
{
    public class HomeController : Controller
    {
        private IRepository repo;
        private InferenceSession _session;
        private float pedestrian = 0.0132F;
        private float bicycle = 0.0088F;
        private float motorcycle = 0.0172F;
        private float imp_restraint = 0.0155F;
        private float unrestrained = 0.0155F;
        private float dui = 0.0243F;
        private float intersection = 0.3937F;
        private float single_vehicle = 0.2380F;
        private float distracted = 0.0908F;

        public HomeController (IRepository temp, InferenceSession session) //
        {
            repo = temp;
            _session = session;
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Crash(int crashseverityid = 0, int pageNum = 1)
        {
            int pageSize = 13;

            var x = new CrashViewModel
            {
                Crash = repo.Crash
                .Where(c => c.crash_severity_id == crashseverityid || crashseverityid == 0)
                .OrderBy(c => c.crash_id)
                .Include(x => x.location)
                .Skip ((pageNum - 1) * pageSize)
                .Take(pageSize),


                PageInfo = new PageInfo
                {
                    TotalNumCrashes = 
                        (crashseverityid == 0
                        ? repo.Crash.Count()
                        : repo.Crash.Where(c => c.crash_severity_id == crashseverityid).Count()),
                    CrashesPerPage = pageSize,
                    CurrentPage = pageNum
                }
            };
            
            return View(x);
        }
        public IActionResult Location(string searchString = "", int pageNum = 1)
        {
            int pageSize = 4;

            var y = new LocationViewModel
            {
                Location = repo.Location
                .OrderBy(l => l.loc_id)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize),

                LocationPageInfo = new LocationPageInfo
                {
                    TotalNumLocations = repo.Location.Count(),
                    LocationsPerPage = pageSize,
                    CurrentPage = pageNum
                }
            };
            if (searchString != "")
            {
                pageSize = 4;
                y.Location = repo.Location.Where(x => x.city == searchString)
                    .Skip((pageNum - 1) * pageSize) //LOCATION SEARCH ONLY RETURNS 1 RECORD per SEARCH PLZ HELP
                    .Take(pageSize);
            };
            
            return View(y);
        }

        public IActionResult LocationDetail(int locid)
        {
            var loc = repo.Location.FirstOrDefault(x => x.loc_id == locid);

            // average scores in each category for the selected city
            var s_pedestrian = repo.Crash.Where(x => x.loc_id == locid).Average(x => x.pedestrian_involved) > pedestrian ? 1.0f : 0.0f;
            var s_bicycle = repo.Crash.Where(x => x.loc_id == locid).Average(x => x.bicyclist_involved) > bicycle ? 1.0f : 0.0f;
            var s_motorcyle = repo.Crash.Where(x => x.loc_id == locid).Average(x => x.motorcycle_involved) > motorcycle ? 1.0f : 0.0f;
            var s_imp_restraint = repo.Crash.Where(x => x.loc_id == locid).Average(x => x.improper_restraint) > imp_restraint ? 1.0f : 0.0f;
            var s_unrestrained = repo.Crash.Where(x => x.loc_id == locid).Average(x => x.unrestrained) > unrestrained ? 1.0f : 0.0f;
            var s_dui = repo.Crash.Where(x => x.loc_id == locid).Average(x => x.dui) > dui ? 1.0f : 0.0f;
            var s_intersection = repo.Crash.Where(x => x.loc_id == locid).Average(x => x.intersection_related) > intersection ? 1.0f : 0.0f;
            var s_singlevehicle = repo.Crash.Where(x => x.loc_id == locid).Average(x => x.single_vehicle) > single_vehicle ? 1.0f : 0.0f;
            var s_distracted = repo.Crash.Where(x => x.loc_id == locid).Average(x => x.distracted_driving) > distracted ? 1.0f : 0.0f;
            var w_jordan = loc.city == "W JORDAN" ? 1.0F : 0.0F;

            // ***START OF CITY SCORE PREDICTION***
            // creates a traffic data(input) object for the overall city score
            var cs_td = new TrafficData
            {
                pedestrian_involved = s_pedestrian,
                bicyclist_involved = s_bicycle,
                motorcycle_involved = s_motorcyle,
                improper_restraint = s_imp_restraint,
                unrestrained = s_unrestrained,
                dui = s_dui,
                intersection_related = s_intersection,
                single_vehicle = s_singlevehicle,
                distracted_driving = s_distracted,
                city_w_jordan = w_jordan
            };

            // creates model for the city score
            var result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", cs_td.AsTensor())
            });
            Tensor<float> score = result.First().AsTensor<float>();
            var cs_prediction = new Prediction { PredictedValue = (float)Math.Round((score.First() * 2), 2) };
            result.Dispose();

            // ***START OF PEDESTRIAN SCORE PREDICTION***
            // creates a traffic data(input) object for the pedestrian city score
            var ps_td = new TrafficData
            {
                pedestrian_involved = 0.0f,
                bicyclist_involved = s_bicycle,
                motorcycle_involved = s_motorcyle,
                improper_restraint = s_imp_restraint,
                unrestrained = s_unrestrained,
                dui = s_dui,
                intersection_related = s_intersection,
                single_vehicle = s_singlevehicle,
                distracted_driving = s_distracted,
                city_w_jordan = w_jordan
            };

            // creates model for the score
            var ps_result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", ps_td.AsTensor())
            });
            Tensor<float> ps_score = ps_result.First().AsTensor<float>();
            var ps_prediction = new Prediction { PredictedValue = (float)Math.Round((ps_score.First() * 2), 2) };
            ps_result.Dispose();

            // ***START OF BICYCLE SCORE PREDICTION***
            // creates a traffic data(input) object for the pedestrian city score
            var bs_td = new TrafficData
            {
                pedestrian_involved = s_pedestrian,
                bicyclist_involved = 0.0f,
                motorcycle_involved = s_motorcyle,
                improper_restraint = s_imp_restraint,
                unrestrained = s_unrestrained,
                dui = s_dui,
                intersection_related = s_intersection,
                single_vehicle = s_singlevehicle,
                distracted_driving = s_distracted,
                city_w_jordan = w_jordan
            };

            // creates model for the score
            var bs_result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", bs_td.AsTensor())
            });
            Tensor<float> bs_score = bs_result.First().AsTensor<float>();
            var bs_prediction = new Prediction { PredictedValue = (float)Math.Round((bs_score.First() * 2), 2) };
            bs_result.Dispose();

            // ***START OF MOTORCYCLE SCORE PREDICTION***
            // creates a traffic data(input) object for the pedestrian city score
            var ms_td = new TrafficData
            {
                pedestrian_involved = s_pedestrian,
                bicyclist_involved = s_bicycle,
                improper_restraint = s_imp_restraint,
                motorcycle_involved = 0.0f,
                unrestrained = s_unrestrained,
                dui = s_dui,
                intersection_related = s_intersection,
                single_vehicle = s_singlevehicle,
                distracted_driving = s_distracted,
                city_w_jordan = w_jordan
            };

            // creates model for the score
            var ms_result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", ms_td.AsTensor())
            });
            Tensor<float> ms_score = ms_result.First().AsTensor<float>();
            var ms_prediction = new Prediction { PredictedValue = (float)Math.Round((ms_score.First() * 2), 2) };
            ms_result.Dispose();

            // ***START OF UNRESTRAINED SCORE PREDICTION***
            // creates a traffic data(input) object for the pedestrian city score
            var us_td = new TrafficData
            {
                pedestrian_involved = s_pedestrian,
                bicyclist_involved = s_bicycle,
                motorcycle_involved = s_motorcyle,
                improper_restraint = s_imp_restraint,
                unrestrained = 0.0f,
                dui = s_dui,
                intersection_related = s_intersection,
                single_vehicle = s_singlevehicle,
                distracted_driving = s_distracted,
                city_w_jordan = w_jordan
            };

            // creates model for the score
            var us_result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", us_td.AsTensor())
            });
            Tensor<float> us_score = us_result.First().AsTensor<float>();
            var us_prediction = new Prediction { PredictedValue = (float)Math.Round((us_score.First() * 2), 2) };
            us_result.Dispose();

            // ***START OF DUI SCORE PREDICTION***
            // creates a traffic data(input) object for the pedestrian city score
            var duis_td = new TrafficData
            {
                pedestrian_involved = s_pedestrian,
                bicyclist_involved = s_bicycle,
                motorcycle_involved = s_motorcyle,
                improper_restraint = s_imp_restraint,
                unrestrained = s_unrestrained,
                dui = 0.0f,
                intersection_related = s_intersection,
                single_vehicle = s_singlevehicle,
                distracted_driving = s_distracted,
                city_w_jordan = w_jordan
            };

            // creates model for the score
            var duis_result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", duis_td.AsTensor())
            });
            Tensor<float> duis_score = duis_result.First().AsTensor<float>();
            var duis_prediction = new Prediction { PredictedValue = (float)Math.Round((duis_score.First() * 2), 2) };
            duis_result.Dispose();

            // ***START OF INTERSECTION SCORE PREDICTION***
            // creates a traffic data(input) object for the pedestrian city score
            var is_td = new TrafficData
            {
                pedestrian_involved = s_pedestrian,
                bicyclist_involved = s_bicycle,
                motorcycle_involved = s_motorcyle,
                improper_restraint = s_imp_restraint,
                unrestrained = s_unrestrained,
                dui = s_dui,
                intersection_related = 0.0f,
                single_vehicle = s_singlevehicle,
                distracted_driving = s_distracted,
                city_w_jordan = w_jordan
            };

            // creates model for the score
            var is_result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", is_td.AsTensor())
            });
            Tensor<float> is_score = is_result.First().AsTensor<float>();
            var is_prediction = new Prediction { PredictedValue = (float)Math.Round((is_score.First() * 2), 2) };
            is_result.Dispose();

            // ***START OF SINGLE VEHICLE SCORE PREDICTION***
            // creates a traffic data(input) object for the pedestrian city score
            var svs_td = new TrafficData
            {
                pedestrian_involved = s_pedestrian,
                bicyclist_involved = s_bicycle,
                motorcycle_involved = s_motorcyle,
                improper_restraint = s_imp_restraint,
                unrestrained = s_unrestrained,
                dui = s_dui,
                intersection_related = s_intersection,
                single_vehicle = 0.0f,
                distracted_driving = s_distracted,
                city_w_jordan = w_jordan
            };

            // creates model for the score
            var svs_result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", svs_td.AsTensor())
            });
            Tensor<float> svs_score = svs_result.First().AsTensor<float>();
            var svs_prediction = new Prediction { PredictedValue = (float)Math.Round((svs_score.First() * 2), 2) };
            svs_result.Dispose();

            // ***START OF DISTRACTED SCORE PREDICTION***
            // creates a traffic data(input) object for the pedestrian city score
            var ds_td = new TrafficData
            {
                pedestrian_involved = s_pedestrian,
                bicyclist_involved = s_bicycle,
                motorcycle_involved = s_motorcyle,
                improper_restraint = s_imp_restraint,
                unrestrained = s_unrestrained,
                dui = s_dui,
                intersection_related = s_intersection,
                single_vehicle = s_singlevehicle,
                distracted_driving = 0.0f,
                city_w_jordan = w_jordan
            };

            // creates model for the score
            var ds_result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", ds_td.AsTensor())
            });
            Tensor<float> ds_score = ds_result.First().AsTensor<float>();
            var ds_prediction = new Prediction { PredictedValue = (float)Math.Round((ds_score.First() * 2), 2) };
            ds_result.Dispose();

            // loads models into the model
            var vm = new CityDetailViewModel
            {
                Location = loc,
                Score = cs_prediction,
                Pedestrian_Prediction = ps_prediction,
                Bicyclist_Prediction = bs_prediction,
                Motorcycle_Prediction = ms_prediction,
                Unrestrained_Prediction = us_prediction,
                DUI_Prediction = duis_prediction,
                Intersection_Prediction = is_prediction,
                SingleVehicle_Prediction = svs_prediction,
                Distracted_Prediction = ds_prediction

            };

            return View(vm); 

        }

    }
}
