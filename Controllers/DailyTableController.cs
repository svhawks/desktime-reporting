using DesktimeReporting.Core;
using DesktimeReporting.Data.Employee;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktimeReporting.Controllers
{
    public class DailyTableController : Controller
    {
        public ActionResult DailyTable(string startDate, string endDate, string manager = "none")
        {

            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);
            List<string> ids = Config.Managers.FirstOrDefault(e => e.Key == manager).Value ?? new List<string>();

            List<Employee> EmployeeDays = EmployeeRepository.Instance.GetDaysBtw(start, end, ids).Result;

            var data = from e in EmployeeDays
                       orderby e.Name, e.Day
                       let overtime = Math.Max(0, e.Desktime - Config.DailyWorkHours * 60 * 60 )
                       let undertime = Math.Max(0, -e.Desktime + Config.DailyWorkHours * 60 * 60 )
                       let overtimeInHours = ((int) overtime / (60 * 60)) + (overtime % (60 * 60) >= 20 * 60 ? 1 : 0)
                       let undertimeInHours = ((int) undertime / (60 * 60)) + (undertime % (60 * 60) > 40 * 60 ? 1 : 0)
                       select new
                       {
                           Name = e.Name,
                           Day = e.Day,
                           TotalDesktime = e.Desktime,
                           TotalDesktimePretty = e.DesktimeToPretty(),
                           TotalDesktimeInHours = e.Desktime / (60*60) + (e.Desktime % (60 * 60) >= 20 * 60 ? 1 : 0),
                           TotalOvertime = overtime,
                           TotalOvertimePretty = Employee.ConvertToHM(overtime),
                           TotalOvertimeInHours = e.Desktime / (60 * 60),
                           TotalUndertime = overtimeInHours,
                           TotalUndertimePretty = Employee.ConvertToHM(undertime),
                           TotalUndertimeInHours = undertimeInHours
                       };
            

            ViewData["manager"] = manager;
            ViewData["start"] = start;
            ViewData["end"] = end;
            return View(data.Select(r => r.ToExpando()));
        }
    }
}
