using Capstone.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Web.DAL
{
    public interface IParkDAO
    {
        List<Park> GetAllParks();
        Park GetPark(string parkCode);
        List<Forecast> GetFiveDayForecast(Park Park);
        bool AddSurvey(Survey survey);
        List<Survey> GetAllSurveys();
        List<string[]> GetFavoriteParks();
    }
}
