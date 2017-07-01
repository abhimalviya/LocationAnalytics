using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocationAnalytics.Models
{
    public class Activity2
    {
        public string type { get; set; }
        public int confidence { get; set; }
    }

    public class Activity
    {
        public string timestampMs { get; set; }
        public List<Activity2> activity { get; set; }
    }

    public class Location
    {
        public string timestampMs { get; set; }
        public int latitudeE7 { get; set; }
        public int longitudeE7 { get; set; }
        public int accuracy { get; set; }
        public List<Activity> activity { get; set; }
    }

    public class RootObject
    {
        public List<Location> locations { get; set; }
    }
}