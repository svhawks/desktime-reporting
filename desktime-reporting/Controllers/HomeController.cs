using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DesktimeReporting.Data.Employee;
using DesktimeReporting.Core;

namespace DesktimeReporting.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string manager = "none")
        {
            ViewData["manager"] = manager;
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
