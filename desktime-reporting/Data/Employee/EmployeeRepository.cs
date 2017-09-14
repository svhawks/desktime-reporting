using DesktimeReporting.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DesktimeReporting.Data.Employee
{

    public class EmployeeRepository : Repository
    {
        private static readonly EmployeeRepository instance = new EmployeeRepository();

        private EmployeeRepository() { }

        public static EmployeeRepository Instance
        {
            get
            {
                return instance;
            }   
        }

        public async Task<Employee> GetEmployeeAsync(string id, DateTime date)
        {
            var url = $"https://desktime.com/api/v2/json/employee?apiKey={Config.ApiKey}&id={id}&date={date.ToString("yyyy-MM-dd")}";
            Employee e = await GetObjectAsync<Employee>(url);
            e.Day = date;
            e.WeekOfYear = date.WeekOfYear();
            e.MonthOfYear = date.Month;
            return e;
        }

        public Employee GetEmployee(string id, DateTime date)
        {
            var url = $"https://desktime.com/api/v2/json/employee?apiKey={Config.ApiKey}&id={id}&date={date.ToString("yyyy-MM-dd")}";
            Employee e = GetObject<Employee>(url);
            e.Day = date;
            e.WeekOfYear = date.WeekOfYear();
            e.MonthOfYear = date.Month;
            return e;
        }

        public List<Employee> GetMonthsBetween(DateTime startDate, DateTime endDate, IEnumerable<string> ids)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, 1);
            endDate = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(1).AddDays(-1);

            List<Employee> EmployeePeriods = new List<Employee>();

            var list = new List<dynamic>();

            var tmpStartDate = Convert.ToDateTime(startDate.ToString());
            do
            {
                list.Add(new { date = tmpStartDate });
                tmpStartDate = tmpStartDate.AddMonths(1);
            } while (tmpStartDate <= endDate);

            Parallel.ForEach(list,
                new ParallelOptions { MaxDegreeOfParallelism = 100 },
                el =>
                {
                    var url = $"https://desktime.com/api/v2/json/employees?apiKey={Config.ApiKey}&date={el.date.ToString("yyyy-MM-dd")}&period=month";
                    var data = GetObject<Dictionary<string, JToken>>(url);

                    var employeesObj = (Dictionary<string, Dictionary<string, Employee>>) JsonConvert.DeserializeObject(data["employees"].ToString(), typeof(Dictionary<string, Dictionary<string, Employee>>));

                    foreach (var dateObj in employeesObj)
                    {
                        if (el.date.Month == Convert.ToDateTime(dateObj.Key).Month) // Convert.ToDateTime(dateObj.Key) >= startDate && 
                        {
                            foreach (var employeeObj in dateObj.Value)
                            {
                                if (ids.Contains(employeeObj.Key))
                                {
                                    employeeObj.Value.MonthOfYear = el.date.Month;
                                    EmployeePeriods.Add(employeeObj.Value);
                                }
                            }
                        }
                    }
                });

            return EmployeePeriods;
        }

        public async Task<List<Employee>> GetDaysBtw(DateTime date, DateTime endDate, IEnumerable<string> ids)
        {
            List<Employee> EmployeeDays = new List<Employee>();
            
            if (ids == null)
            {
                var url = $"https://desktime.com/api/v2/json/employees?apiKey={Config.ApiKey}&date={date.ToString("yyyy-MM-dd")}";
                var data = await GetJsonAsync(url);
                ids = data["employees"].Select(emp => emp.First["id"].ToString());
            }

            var list = new List<dynamic>();

            do
            {
                foreach (var id in ids)
                {
                    list.Add(new { id = id, date = date });
                }
                date = date.AddDays(1);
            } while (date <= endDate);


            Parallel.ForEach(list,
                new ParallelOptions { MaxDegreeOfParallelism = 100 },
                el => {
                    EmployeeDays.Add(EmployeeRepository.Instance.GetEmployee(el.id, el.date));
                });


            return EmployeeDays;
        }
    }
}