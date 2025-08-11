using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Data;
using ScheduleApp.Models;
using System.Linq;

namespace ScheduleApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationContext context;
        private readonly JSONDeserializer scheduleService;
        public AdminController(ApplicationContext context, JSONDeserializer scheduleService)
        {

            this.context = context;
            this.scheduleService = scheduleService;
        }
        private const string AdminPassword = "1111";

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string password)
        {
            if (password == AdminPassword)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Dashboard");
            }
            ViewBag.Error = "Неправильний пароль!";
            return View();
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public IActionResult AddData(SaturdayClass saturdaylass)
        {
            saturdaylass.StartSaturday = saturdaylass.StartSaturday.Date.ToUniversalTime();
            saturdaylass.EndSaturday = saturdaylass.EndSaturday.Date.ToUniversalTime();

            context.SaturdayClasses.Add(saturdaylass);
            context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        public IActionResult Delete()
        {
            SaturdayClass s = context.SaturdayClasses.FirstOrDefault();
            if(s != null)
            {
                context.Remove(s);
                context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
        public IActionResult UppdateDatabase()
        {
            context.Schedules.RemoveRange(context.Schedules);
            context.ClassItems.RemoveRange(context.ClassItems);
            context.Lessons.RemoveRange(context.Lessons);
            context.SemesterClasses.RemoveRange(context.SemesterClasses);
            context.Days.RemoveRange(context.Days);
            context.Weeks.RemoveRange(context.Weeks);
            context.Groups.RemoveRange(context.Groups);
            context.Rooms.RemoveRange(context.Rooms);
            context.Departments.RemoveRange(context.Departments);
            context.Semesters.RemoveRange(context.Semesters);
            context.Teachers.RemoveRange(context.Teachers);
            context.Roots.RemoveRange(context.Roots);

            context.SaveChanges();
            var scheduleData = scheduleService.GetScheduleData().Result;
            if (scheduleData != null)
            {
            context.Add(scheduleData);
            context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

    }
}
