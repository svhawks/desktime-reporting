using DesktimeReporting.Core;
using DesktimeReporting.Data.Employee;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktimeReporting.Controllers
{
    public class WeeklyTableController : Controller
    {

        public ActionResult WeeklyTable(string startDate, string endDate, string manager = "none")
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);
            List<string> ids = Config.Managers.FirstOrDefault(e => e.Key == manager).Value ?? new List<string>();

            if (start == null && end == null)
            {
                end = DateTime.Today.GetFirstDayOfWeek();
                start = end.AddDays(-7).AddDays(1);
            }
            List<Employee> EmployeeDays = EmployeeRepository.Instance.GetDaysBtw(start, end, ids).Result;
            //Desktime.PrintDaily();

            var grouped = from empDay in EmployeeDays
                          group empDay by new { empDay.Name, empDay.WeekOfYear } into w
                          let weekEndDate  = DateTime.Today < w.First().Day.GetFirstDayOfWeek().AddDays(1).AddDays(6) ? DateTime.Today : w.First().Day.GetFirstDayOfWeek().AddDays(1).AddDays(6)
                          let weekStartDate = w.First().Day.GetFirstDayOfWeek().AddDays(1)
                          let desktime = w.Sum(_ => _.Desktime)
                          let overtime = Math.Max(0, desktime - Config.DailyWorkHours * 60 * 60 * Math.Min(5, 1 + (weekEndDate - weekStartDate).TotalDays))
                          let undertime = Math.Max(0, -desktime + Config.DailyWorkHours * 60 * 60 * Math.Min(5, 1 + (weekEndDate - weekStartDate).TotalDays))
                          let overtimeInHours = ((int) overtime / (60 * 60)) + (overtime % (60 * 60) >= 20 * 60 ? 1 : 0)
                          let undertimeInHours = ((int) undertime / (60 * 60)) + (undertime % (60 * 60) > 40 * 60 ? 1 : 0)
                          select new
                          {
                              Name = w.First().Name,
                              Week = w.First().WeekOfYear,
                              WeekStartDate = weekStartDate,
                              WeekEndDate = weekEndDate,
                              TotalDesktime = desktime.ToString(),
                              TotalDesktimePretty = Employee.ConvertToHM(desktime),
                              TotalDesktimeInHours = desktime / (60*60) + (desktime % (60 * 60) >= 20 * 60 ? 1 : 0),
                              TotalOvertime = overtime,
                              TotalOvertimePretty = Employee.ConvertToHM(overtime),
                              TotalOvertimeInHours = overtimeInHours,
                              TotalUndertime = undertime,
                              TotalUndertimePretty = Employee.ConvertToHM(undertime),
                              TotalUndertimeInHours = undertimeInHours,
                              TotalDays = w.Count().ToString(),
                          };



            grouped = from g in grouped
                      orderby g.Name, g.Week
                      select g;

            var results = grouped.Select(r => r.ToExpando());

            ViewData["manager"] = manager;
            ViewData["start"] = start;
            ViewData["end"] = end;
            return View(results);
        }

    }
}
