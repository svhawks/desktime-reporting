using DesktimeReporting.Core;
using DesktimeReporting.Data.Employee;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktimeReporting.Controllers
{
    public class MonthlyGraphController : Controller
    {
        public ActionResult MonthlyGraph(DateTime startDate, DateTime endDate, string manager = "none")
        {
            List<string> ids = Config.Managers.FirstOrDefault(e => e.Key == manager).Value ?? new List<string>();


            List<Employee> EmployeeDays = EmployeeRepository.Instance.GetMonthsBetween(startDate, endDate, ids);


            var grouped = from empDay in EmployeeDays
                          group empDay by new { empDay.Name, empDay.MonthOfYear } into w
                          let desktime = w.Sum(_ => _.Desktime)
                          select new
                          {
                              Name = w.First().Name,
                              Month = w.First().MonthOfYear,
                              Desktime = desktime.ToString(),
                              Overtime = Math.Max(0, desktime - 60 * 60 * 160),
                              DesktimePretty = Employee.ConvertToHM(w.Sum(_ => _.Desktime)),
                              OvertimePretty = Employee.ConvertToHM(Math.Max(0, w.Sum(_ => _.Desktime) - 60 * 60 * 160))
                          };



            grouped = from g in grouped
                      orderby g.Name, g.Month
                      select g;

            var results = grouped.Select(r => r.ToExpando());

            ViewData["manager"] = manager;
            return View(results);
        }

    }
}
