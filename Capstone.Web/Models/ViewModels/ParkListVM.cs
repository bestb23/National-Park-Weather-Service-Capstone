using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Web.Models.ViewModels
{
    public class ParkListVM
    {
        public List<Park> Parks { get; set; }
        public string parkCode { get; set; }
    }
}
