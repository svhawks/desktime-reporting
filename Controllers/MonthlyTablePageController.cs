using DesktimeReporting.Core;
using DesktimeReporting.Data.Employee;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktimeReporting.Controllers
{
    public class MonthlyTablePageController : Controller
    {
        public ActionResult MonthlyTablePage(DateTime startDate, DateTime endDate, string manager = "none")
        {
            List<string> ids = Config.Managers.FirstOrDefault(e => e.Key == manager).Value ?? new List<string>();

            List<Employee> EmployeeDays = EmployeeRepository.Instance.GetMonthsBetween(startDate, endDate, ids);

            var grouped = from empDay in EmployeeDays
                          group empDay by new { empDay.Name } into w
                          select new
                          {
                              Name = w.First().Name,
                              MonthlyDesktimes = w.GroupBy(e => e.MonthOfYear).ToDictionary(e => e.First().MonthOfYear, e => e.Sum(x => x.Desktime)),
                              TotalDays = w.Count().ToString(),
                          };



            grouped = from g in grouped
                      orderby g.Name 
                      select g;

            var results = grouped.Select(r => r.ToExpando());

            ViewData["quarterNum"] = endDate.Month / 3;
            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;
            ViewData["manager"] = manager;
            return View(results);
        }

    }
}
