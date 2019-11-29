using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Capstone.Web.Models;
using Capstone.Web.DAL;
using Capstone.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Routing;

namespace Capstone.Web.Controllers
{
    public class HomeController : Controller
    {
        public IParkDAO ParkDAO;
        public HomeController(IParkDAO dao)
        {
            ParkDAO = dao;
        }

        public IActionResult Index()
        {
            List<Park> parks = ParkDAO.GetAllParks();
            ParkListVM vm = new ParkListVM()
            {
                Parks = parks
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Detail(string parkCode)
        {
            Park park = ParkDAO.GetPark(parkCode);
            List<Forecast> forecasts = ParkDAO.GetFiveDayForecast(park);
            DetailVM vm = new DetailVM()
            {
                Park = park,
                Forecasts = forecasts,
                TempUnits = GetSessionTempUnits()
            };
            foreach (Forecast forecast in vm.Forecasts)
            {
                forecast.TempUnits = vm.TempUnits;
            }
            // store the park and forecasts
            //string jsonVM = JsonConvert.SerializeObject(vm);
            //HttpContext.Session.SetString("detailVM", jsonVM);
            TempData["parkCode"] = vm.Park.ParkCode;

            return View(vm);
        }
        [HttpPost]
        public IActionResult Detail(DetailVM vm)
        {
            // this used session more directly than tempdata, and wasn't a post-redirect-get
            //    |   |   |   |   |   |   |   |   |   | 
            //    v   v   v   v   v   v   v   v   v   v 
                //// Get session stored parks and forecasts
                //string jsonVM = HttpContext.Session.GetString("detailVM");
                //DetailVM sessionVM = JsonConvert.DeserializeObject<DetailVM>(jsonVM);
                //vm.Park = sessionVM.Park;
                //vm.Forecasts = sessionVM.Forecasts;
                //// set temp units
                //SetSessionTempUnits(vm.TempUnits);
                //foreach (Forecast forecast in vm.Forecasts)
                //{
                //    forecast.TempUnits = vm.TempUnits;
                //}
                //return View(vm);
            if (vm.TempUnits != "C")
            {
                vm.TempUnits = "F";
            }
            HttpContext.Session.SetString("tempUnits", vm.TempUnits);
            RouteValueDictionary routeDict = new RouteValueDictionary() { { "parkCode", TempData["parkCode"] } };
            return RedirectToAction("Detail", routeDict);
        }

        [HttpGet]
        public IActionResult Survey()
        {
            SurveyFormVM vm = new SurveyFormVM();
            foreach(Park park in ParkDAO.GetAllParks())
            {
                SelectListItem item = new SelectListItem(park.ParkName, park.ParkCode);
                vm.Parks.Add(item);
            }
            return View(vm);
        }
        [HttpPost]
        public IActionResult Survey(SurveyFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                foreach (Park park in ParkDAO.GetAllParks())
                {
                    SelectListItem item = new SelectListItem(park.ParkName, park.ParkCode);
                    vm.Parks.Add(item);
                }
                return View(vm);
            }
            ParkDAO.AddSurvey(vm.Survey);

            return RedirectToAction("Favorites");
        }


        public IActionResult Favorites()
        {
            List<string[]> votesList = ParkDAO.GetFavoriteParks();
            return View(votesList);
        }

        string GetSessionTempUnits()
        {
            if (HttpContext.Session.GetString("tempUnits") == null)
            {
                return "F";
            }
            else
            {
                return HttpContext.Session.GetString("tempUnits");
            }
        }
        void SetSessionTempUnits(string tempUnits)
        {
            HttpContext.Session.SetString("tempUnits", tempUnits);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
