using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Transactions;
using System.Data.SqlClient;
using Capstone.Web.DAL;
using System.Collections.Generic;
using Capstone.Web.Models;
using System;
using Microsoft.SqlServer;

namespace UnitTests
{
    [TestClass]
    public class DAOTests
    {
        public TransactionScope transaction;
        const string connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=NPGeek;Integrated Security=True;";
        ParkSqlDAO DAO;
        [TestInitialize]
        public void Setup()
        {
            transaction = new TransactionScope();
            string script;
            using (StreamReader sr = new StreamReader("npgeek.sql"))
            {
                script = sr.ReadToEnd();
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                SqlCommand cmd = new SqlCommand(script, conn);
                conn.Open();

                SqlDataReader data = cmd.ExecuteReader();
            }
            DAO = new ParkSqlDAO(connectionString);
        }
        [TestCleanup]
        public void Cleanup()
        {
            transaction.Dispose();
        }

        [TestMethod]
        public void GetPark()
        {
            Park park = DAO.GetPark("CVNP");
            Assert.AreEqual("Cuyahoga Valley National Park",park.ParkName, "ParkDAO should pull the correct park.");
            park = DAO.GetPark("MRNP");
            Assert.AreEqual("Mount Rainier National Park", park.ParkName, "ParkDAO should pull the correct park.");
        }
        [TestMethod]
        public void GetAllParks()
        {
            List<Park> parks = DAO.GetAllParks();

            Assert.AreEqual(10, parks.Count, "There should be 10 parks.");
            Assert.AreEqual(parks[0].ParkName, "Cuyahoga Valley National Park", "Parks should be alphabetical");
            Assert.AreEqual(parks[9].ParkName, "Yosemite National Park", "Parks should be alphabetical");
            Assert.IsFalse(parks.Contains(null), "All parks should have information");
        }

        [TestMethod]
        public void Surveys()
        {
            Survey survey1 = new Survey()
            {
                ParkCode = "ENP",
                EmailAddress = "fake@gmail.com",
                State = "Florida",
                ActivityLevel = "sedentary"
            };
            Survey survey2 = new Survey()
            {
                ParkCode = "CVNP",
                EmailAddress = "bogus@hotmail.com",
                State = "Ohio",
                ActivityLevel = "active"
            };
            Survey survey3 = new Survey()
            {
                ParkCode = "MRNP",
                EmailAddress = "fugazi@yahoo.com",
                State = "Washington",
                ActivityLevel = "extremely active"
            };

            try
            {
                DAO.AddSurvey(survey1);
                DAO.AddSurvey(survey2);
                DAO.AddSurvey(survey3);
                Assert.IsTrue(true);
            }
            catch (Exception e)
            {
                Assert.IsFalse(true, e.Message);
            }

            List<Survey> surveys = DAO.GetAllSurveys();
            Assert.AreEqual("fake@gmail.com", surveys[0].EmailAddress);
            Assert.AreEqual(3, surveys.Count, "3 surveys should have been added.");
        }
        [TestMethod]
        public void TestGetFiveDayForcast()
        {
            Park park = DAO.GetPark("CVNP");
            List<Forecast> forecasts = DAO.GetFiveDayForecast(park);

            Assert.AreEqual(5, forecasts.Count, "There are not 5 days in the forecast");
            Assert.AreEqual(1, forecasts[0].FiveDayForecastValue, "First forecast day is not day 1");
            Assert.AreEqual(38, forecasts[1].LowInF, "Low Temp for day 2 did not return 38");
            Assert.AreEqual(66, forecasts[2].HighInF, "High temo for day 3 did not return 66");
            Assert.AreEqual("rain", forecasts[3].Weather, "weather for day 4 didnt return rain");
        }
    }
}
