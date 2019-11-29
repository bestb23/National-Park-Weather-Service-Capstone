using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Web.Models
{
    public class Survey
    {
        public int SurveyId { get; set; }
        [Required(ErrorMessage = "Please select your favorite park.")]
        public string ParkCode { get; set; }
        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Please select your state.")]
        public string State { get; set; }
        [Required(ErrorMessage = "Please select your activity level.")]
        public string ActivityLevel { get; set; }
    }
}
