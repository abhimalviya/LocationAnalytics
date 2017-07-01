using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LocationAnalytics.Models;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace LocationAnalytics.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            UploadData();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private JsonResult UploadData()
        {
            var rooObject = LoadJson();
            #region commented
            //if (rooObject != null)
            //{
            //     saveDataInDB(rooObject, "abhishek");
            //}
            //Cluster cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            //ISession session = cluster.Connect("locationsks");

            //Row result = session.Execute("select * from locations where userName='abhishek'").First();
            #endregion
            createFiles(rooObject);
            return Json(rooObject, JsonRequestBehavior.AllowGet);
        }

        private RootObject LoadJson()
        {
            using (StreamReader r = new StreamReader(@"C:\Users\admin1\Downloads\takeout-20170701T041322Z-001\Takeout\Location History\Location History.json"))
            {
                using (JsonTextReader reader = new JsonTextReader(r))
                {
                    var serializer = new JsonSerializer();

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            return serializer.Deserialize<RootObject>(reader);
                        }
                    }
                }
            }

            return null;
        }

        private void createFiles(RootObject rootobject)
        {
            rootobject.locations.Where(q => checkConditions(q.timestampMs, rootobject.locations.Count)).ToList();
            var rers=Split(rootobject.locations);
        }

        bool checkConditions(string timestampMs,int length)
        {
            return false;
        }

        public static IEnumerable<IEnumerable<Location>> Split(IEnumerable<Location> list)
        {
            int i = 0;
            int parts = 0;
            if (list.Count() > 100000)
            {
                parts = 10;
            }
            var splits = from item in list
                         group item by i++ % parts into part
                         select part.AsEnumerable();
            return splits;
        }

        private void saveDataInDB(RootObject data, string userName)
        {


            Cluster cluster = Cluster.Builder().AddContactPoints("127.0.0.1").Build();
            ISession session = cluster.Connect("locationsks");

            Parallel.ForEach(data.locations, loc =>
            {
                var activityJson = String.Empty;
                if (loc.activity != null)
                    activityJson = JsonConvert.SerializeObject(loc.activity);

                string insertQuery = @"insert into locations (timestampcol, username, latitude, longitude, accuracy, activityJson) 
                                  values ( '" + loc.timestampMs + "', '"
                                  + userName + "', '"
                                  + loc.latitudeE7.ToString() + "', '"
                                  + loc.longitudeE7.ToString() + "', "
                                  + loc.accuracy + ", '"
                                  + activityJson + "')";

                session.Execute(insertQuery);
            });

        }
    }
}