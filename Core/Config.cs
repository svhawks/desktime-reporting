using DesktimeReporting.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktimeReporting.Core
{

    public static class Config
    {
        

        public static string ApiKey { get; } = Resources.ResourceManager.GetString("API_KEY");

        private static string resManagers = Resources.ResourceManager.GetString("MANAGERS");

        public static Dictionary<string, List<string>> Managers { get; } = resManagers.Split(';').Select(e => e.Trim()).ToDictionary(
            e => e.Split(':')[0],
            e => e.Split(':')[1].Split(',').ToList<string>()
        );
            
        public static double DailyWorkHours = double.Parse(Resources.ResourceManager.GetString("DAILY_WORK_HOURS"));
   }

}
