using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Capstone.Web.Models;

namespace Capstone.Web.DAL
{
    public class ParkSqlDAO : IParkDAO
    {
        public string ConnectionString;
        public ParkSqlDAO(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public Park GetPark(string parkCode)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string sqlQuery = "select * from park where parkCode = @code";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@code", parkCode);

                conn.Open();
                SqlDataReader data = cmd.ExecuteReader();
                data.Read();
                return ConvertRowToPark(data);
            }
        }
        public List<Park> GetAllParks()
        {
            List<Park> parks = new List<Park>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string sql = "select * from park order by parkName";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    Park park = ConvertRowToPark(reader);
                    parks.Add(park);
                }
            }
            return parks;
        }
        public List<Forecast> GetFiveDayForecast(Park Park)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sqlQuery = @"select * from weather where parkCode = @parkCode";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@parkCode", Park.ParkCode);

                SqlDataReader data = cmd.ExecuteReader();
                List<Forecast> Forecasts = new List<Forecast>();
                while (data.Read())
                {
                    Forecasts.Add(ConvertRowToForecast(data));
                }
                return Forecasts;
            }
        }
        public List<Survey> GetAllSurveys()
        {
            List<Survey> surveys = new List<Survey>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string sql = "select * from survey_result";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Survey survey = new Survey();
                    survey.ActivityLevel = Convert.ToString(reader["activityLevel"]);
                    survey.EmailAddress = Convert.ToString(reader["emailAddress"]);
                    survey.ParkCode = Convert.ToString(reader["parkCode"]);
                    survey.State = Convert.ToString(reader["state"]);
                    survey.SurveyId = Convert.ToInt32(reader["surveyId"]);

                    surveys.Add(survey);
                }
            }
            return surveys;
        }
        public bool AddSurvey(Survey survey)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    string sqlQuery = @"INSERT INTO survey_result (parkCode, emailAddress, state, activityLevel)
                                        VALUES (@parkCode, @emailAddress, @state, @activityLevel)";
                    SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                    cmd.Parameters.AddWithValue("@parkCode", survey.ParkCode);
                    cmd.Parameters.AddWithValue("@emailAddress", survey.EmailAddress);
                    cmd.Parameters.AddWithValue("@state", survey.State);
                    cmd.Parameters.AddWithValue("@activityLevel", survey.ActivityLevel);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch(SqlException e)
            {
                throw e;
                //return false;
            }
        }

        public Park ConvertRowToPark(SqlDataReader data)
        {
            return new Park()
            {
                ParkCode = Convert.ToString(data["parkCode"]),
                ParkName = Convert.ToString(data["parkName"]),
                State = Convert.ToString(data["state"]),
                Acreage = Convert.ToInt32(data["acreage"]),
                ElevationInFeet = Convert.ToInt32(data["elevationInFeet"]),
                MilesOfTrail = Convert.ToDouble(data["milesOfTrail"]),
                NumberOfCampsites = Convert.ToInt32(data["numberOfCampsites"]),
                Climate = Convert.ToString(data["climate"]),
                YearFounded = Convert.ToInt32(data["yearFounded"]),
                AnnualVisitorCount = Convert.ToInt32(data["annualVisitorCount"]),
                InspirationalQuote = Convert.ToString(data["inspirationalQuote"]),
                InspirationalQuoteSource = Convert.ToString(data["inspirationalQuoteSource"]),
                ParkDescription = Convert.ToString(data["parkDescription"]),
                EntryFee = Convert.ToInt32(data["entryFee"]),
                NumberOfAnimalSpecies = Convert.ToInt32(data["numberOfAnimalSpecies"])
            };
        }
        public Forecast ConvertRowToForecast (SqlDataReader data)
        {
            Forecast result =  new Forecast("F")
            {
                ParkCode = Convert.ToString(data["parkCode"]),
                FiveDayForecastValue = Convert.ToInt32(data["fiveDayForecastValue"]),
                HighInF = Convert.ToDouble(data["high"]),
                LowInF = Convert.ToDouble(data["low"]),
                Weather = Convert.ToString(data["forecast"])
            };

            if (result.Weather == "partly cloudy")
            {
                result.Weather = "partlyCloudy";
            }

            return result;
        }
        public List<string[]> GetFavoriteParks()
        {
            List<string[]> result = new List<string[]>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string sql = @"select sr.parkCode, p.parkName, count(sr.parkCode) 'votes' from survey_result sr
join park p on p.parkCode = sr.parkCode
group by sr.parkCode, p.parkName order by count(sr.parkCode) desc, p.parkName";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //if(!result.TryAdd(Convert.ToString(reader["parkCode"]),Convert.ToInt32(reader["votes"])))
                    //{
                    //    result[Convert.ToString(reader["parkCode"])]++;
                    //}
                    result.Add(ConvertRowToArray(reader));
                }
            }
            return result;
        }
        private string[] ConvertRowToArray(SqlDataReader reader)
        {
            return new string[] { Convert.ToString(reader["parkCode"]), Convert.ToString(reader["parkName"]), Convert.ToString(reader["votes"]) };
        }
    }
}
