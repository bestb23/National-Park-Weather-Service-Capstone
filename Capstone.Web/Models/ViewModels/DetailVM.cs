using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Web.Models.ViewModels
{
    public class DetailVM
    {
        public Park Park { get; set; }
        public List<Forecast> Forecasts { get; set; }
        public string TempUnits { get; set; }
    }
}
