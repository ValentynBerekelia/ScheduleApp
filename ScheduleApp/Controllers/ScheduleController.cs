﻿using Azure;
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

        //Зробити підсвітку конкретного дня/можливо конкретної пари
        [HttpGet]
        public IActionResult GetTeacher(string surname,string group,DateTime date)
        {
            if (group != null && surname == null && date.Year < 2000)
            {
                var result = _rootRepository.Group(group);
                return RedirectToAction("getGroup", "Group", new { group });
            }
            else if(group == null && surname != null && date.Year < 2000)
            {
                var result = _rootRepository.SearchForTeacher(surname);
                return View("GetTeacher", result);
            }
            else if(group != null && date.Year >= 2024 && surname == null)
            {
                var result = _rootRepository.SearchByDate(group, date);
                return View("GetTeacher", result);
            }
            return View("Error");
        }
    }
}
