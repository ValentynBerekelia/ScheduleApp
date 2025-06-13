using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ScheduleApp.Models;
using ScheduleApp.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
namespace ScheduleApp.Controllers
{
    //[ViewComponent]
    public class ScheduleController : Controller
    {
        private readonly ScheduleService _rootRepository;
        public ScheduleController(ScheduleService rootRepository)
        {
            _rootRepository = rootRepository;
        }
        public IActionResult Index()
        {
            var list = _rootRepository.TeacherList();
            return View(list);
        }

        [HttpGet]
        public IActionResult GetSchedule(string surname,string group,DateTime date)
        {
            if (group != null && surname == null && date.Year < 2000)
            {
                var listTeacher = _rootRepository.TeacherList();
                var result = _rootRepository.SearchByGroup(group);
                var model = new ViewModel { TeacherInfos = listTeacher, LesoInfos = result };
                return View("GetSchedule", model);
            }
            else if (group == null && surname != null && date.Year < 2000)
            {
                var listTeacher = _rootRepository.TeacherList();
                var result = _rootRepository.SearchForTeacher(surname);
                var model = new ViewModel { TeacherInfos = listTeacher, LesoInfos = result };
                return View("GetSchedule", model);
            }
            else if (group != null && date.Year >= 2024 && surname == null)
            {
                var listTeacher = _rootRepository.TeacherList();
                var result = _rootRepository.SearchByDate(group, date);
                var model = new ViewModel { TeacherInfos = listTeacher, LesoInfos = result };
                return View("GetSchedule", model);
            }
            else if (surname != null && date.Year >= 2025)
            {
                var listTeacher = _rootRepository.TeacherList();
                var result = _rootRepository.TeacherAndDate(surname, date);
                var model = new ViewModel { TeacherInfos = listTeacher, LesoInfos = result };
                return View("GetSchedule", model);
            }
            else
            {
                var listTeacher = _rootRepository.TeacherList();
                var error = new List<LessonInfo> { new LessonInfo { Day = "" } };
                var model = new ViewModel { TeacherInfos = listTeacher, LesoInfos = error };
                return View("GetSchedule", model);
            }
        }
    }
}
