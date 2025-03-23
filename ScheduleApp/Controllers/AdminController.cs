using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Models;
using ScheduleApp.Services;
using System.Linq;

namespace ScheduleApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationContext context;

        public AdminController(ApplicationContext context)
        {
            this.context = context;
        }
        private const string AdminPassword = "1634532h";

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
        public IActionResult AddData(SaturdayClass saturdayclass)
        {
            context.SaturdayClasses.Add(saturdayclass);
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
  
    }
}
