using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Models;
using System;
using System.Data;
using System.Data.Common;
namespace ScheduleApp.Controllers
{
    public class GroupController : Controller
    {
        private readonly ScheduleService _rootRepository;
        public GroupController(ScheduleService rootRepository)
        {
            _rootRepository = rootRepository;
        }


        [HttpGet]
        public IActionResult GetGroup(string group)
        {
            var result = _rootRepository.Group(group);
            return View(result);
        }
    }
}
