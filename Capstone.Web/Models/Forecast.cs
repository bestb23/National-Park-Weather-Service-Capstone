using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Web.Models
{
    public class Forecast
    {
        public Forecast(string tempUnits)
        {
            if (tempUnits.ToLower() == "f" || tempUnits.ToLower() == "fahrenheit")
            {
                TempUnits = "F";
            }
            else if (tempUnits.ToLower() == "c" || tempUnits.ToLower() == "celsius")
            {
                TempUnits = "C";
            }
            else
            {
                throw new ArgumentException("Cannot create Forecast; invalid temperature units given.");
            }
        }

        public string TempUnits { get; set; }
        public string ParkCode { get; set; }
        public int FiveDayForecastValue { get; set; } // fiveDayForecastValue
        public string Weekday
        {
            get
            {
                DateTime weekdayDT = DateTime.Now + new TimeSpan(24 * (FiveDayForecastValue - 1), 0, 0);
                
                return weekdayDT.DayOfWeek.ToString();
            }
        }

        public int High => TempUnits == "F" ? (int)HighInF : (int)TempInC(HighInF);
        public int Low => TempUnits == "F" ? (int)LowInF : (int)TempInC(LowInF);
        public double HighInF { get; set; }
        public double LowInF { get; set; }
        public string Weather { get; set; }
        public string WeatherProperNoun
        {
            get
            {
                if (Weather == "partlyCloudy")
                {
                    return "Partly Cloudy";
                }
                string lowercase = Weather;
                List<string> list = lowercase.Split(" ").ToList<string>();
                string propercased = "";
                foreach (string word in list)
                {
                    string newWord = word.Substring(1);
                    newWord = word[0].ToString().ToUpper() + newWord;
                    propercased += newWord + " ";
                }
                propercased = propercased.Substring(0, propercased.Count() - 1);
                return propercased;
            }
        }
        public string WeatherRecommendation
        {
            get
            {
                switch(Weather.ToLower())
                {
                    case "snow":
                        return "Pack snowshoes.";
                    case "rain":
                        return "Pack rain gear and wear waterproof shoes.";
                    case "thunderstorms":
                        return "Seek shelter and avoid hiking on exposed ridges.";
                    case "sunny":
                        return "Pack sunblock.";
                    default:
                        return null;
                }
            }
        }
        public string HighTempRecommendation => HighInF > 75 ? "Bring an extra gallon of water." : null;
        public string LowTempRecommendation => LowInF < 20 ? "Be careful of exposure to frigid temperatures." : null;
        public string DiffTempRecommendation => (HighInF - LowInF) > 20 ? "Wear breathable layers." : null;

        public double TempInF(double tempInF)
        {
            if (TempUnits == "F")
            {
                return tempInF;
            }
            else
            {
                throw new ArgumentException("Invalid temp units.");
            }
        }
        public double TempInC(double tempInF)
        {
            if (TempUnits == "C")
            {
                return (tempInF - 32) / 1.8;
            }
            else
            {
                throw new ArgumentException("Invalid temp units.");
            }
        }
    }
}
