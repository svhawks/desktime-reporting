using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DesktimeReporting.Data.Employee
{

    public class Employee
    {

        public Employee(string id, DateTime day, string name = "", int desktime = 0)
        {
            this.Id = id;
            this.Name = name;
            this.Desktime = desktime;
            this.Day = day;
            this.WeekOfYear = day.WeekOfYear();
            this.MonthOfYear = day.Month;
        }

        public Employee()
        {

        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("desktimeTime")]
        public int Desktime { get; set; }
        
        public DateTime Day { get; set; }

        public int WeekOfYear { get; set; }

        public int MonthOfYear { get; set; }

        public int WeeklySafeZone()
        {
            int weekly = 40;

            return weekly;
        }

        public string DesktimeToPretty()
        {
            return Employee.ConvertToHM(Desktime);
        }

        public static string ConvertToHM(object value)
        {
            int Desktime = Int32.Parse(value.ToString());
            return $"{Desktime / 3600}h {(Desktime - ((Desktime / 3600) * 3600)) / 60}m";
        }
    }

}