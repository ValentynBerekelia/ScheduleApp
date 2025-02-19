using Azure;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Serialization;
using ScheduleApp.Models;
using ScheduleApp.Services;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class RootRepository
{
    private readonly ApplicationContext context;

    public RootRepository(ApplicationContext context)
    {
        this.context = context;
    }


    public DbSet<Root> GetAllTable()
    {
        context.Roots.Load();
        context.Semesters.Load();
        context.Schedules.Load();
        context.Groups.Load();
        context.Days.Load();
        context.ClassItems.Load();
        context.SemesterClasses.Load();
        context.Weeks.Load();
        context.Teachers.Load();
        context.Departments.Load();
        context.Lessons.Load();
        context.Rooms.Load();
        return context.Roots;
    }

    public List<TeacherInfo> TeacherList()
    {
        var teacher = context.Roots
              .SelectMany(r => r.Schedule)
              .SelectMany(s => s.Days, (schedule, day) => new { schedule, day })
              .SelectMany(sd => sd.day.Classes, (sd, classItem) => new { sd.schedule, sd.day, classItem })
              .Where(s=>s.classItem.Weeks.Even.Teacher.Surname != null)
              .Select(s => new TeacherInfo
              {
                  TeacherName = s.classItem.Weeks.Even.Teacher.Name,
                  TeacherSurname = s.classItem.Weeks.Even.Teacher.Surname,
                  TeacherPatronymic = s.classItem.Weeks.Even.Teacher.Patronymic,
              }).Distinct().ToList();
        var group = context.Roots
      .SelectMany(r => r.Schedule)
      .SelectMany(s => s.Days, (schedule, day) => new { schedule, day })
      .SelectMany(sd => sd.day.Classes, (sd, classItem) => new { sd.schedule, sd.day, classItem })
      .Where(s => s.classItem.Weeks.Even.Teacher.Surname != null)
      .Select(s => new TeacherInfo
      {
        GroupName = s.schedule.Group.Title
      }).Distinct().ToList();
        teacher.AddRange(group);
        return teacher;   
    }


    //ScheduleController
    public List<LessonInfo> SearchForTeacher(string surname)
    {
        //перевірка на парність/непарність тижня
        var lesson = context.Roots
              .SelectMany(r => r.Schedule)
              .SelectMany(s => s.Days, (schedule, day) => new { schedule, day })
              .SelectMany(sd => sd.day.Classes, (sd, classItem) => new { sd.schedule, sd.day, classItem })
              .Where(s => s.classItem.Weeks.Even.Teacher.Surname + " " + s.classItem.Weeks.Even.Teacher.Name + " " + s.classItem.Weeks.Even.Teacher.Patronymic == surname)
              .GroupBy(g => new
              {
                  g.classItem.Weeks.Even.Teacher.Name,
                  g.classItem.Weeks.Even.Teacher.Surname,
                  g.classItem.Weeks.Even.Teacher.Patronymic,
                  g.day.day,
                  g.classItem.Weeks.Even.SubjectForSite,
                  g.classItem.Class.EndTime,
                  g.classItem.Class.StartTime,
                  g.classItem.Weeks.Even.LessonType,
                  RoomName = g.classItem.Weeks.Even.Room.Name
              })
              .Select(g => new LessonInfo
              {
                  Day = g.Key.day,
                  TeacherName = g.Key.Name,
                  TeacherPatronymic = g.Key.Patronymic,
                  TeacherSurname = g.Key.Surname,
                  GroupTitle = string.Join(", ", g.Select(x => x.schedule.Group.Title)),
                  Subject = g.Key.SubjectForSite,
                  EndTime = g.Key.EndTime,
                  StartTime = g.Key.StartTime,
                  LessonType = g.Key.LessonType,
                  RoomName = g.Key.RoomName
              }).OrderBy(x=>x.StartTime).ToList();

        return lesson;
    }

    //GroupController
    public static int WeekСhecker(DateTime Start, DateTime Today)
    {
        int count = 0;//1
        double var2 = Today.DayOfYear - Start.DayOfYear;
        for (int i = 0; i < var2; i++)
        {
            DateTime timeNext = Start.AddDays(i);
            if (timeNext.DayOfWeek == DayOfWeek.Sunday)
            {
                count += 1;
            }
        }
        return count;
    }
    public List<LessonInfo> OddWeekScheduleOutput(string group)
    {
        var roots = GetAllTable();
        var listResult = context.Roots
            .SelectMany(r => r.Schedule)
            .SelectMany(s => s.Days, (schedule, day) => new { schedule, day })
            .SelectMany(sd => sd.day.Classes, (sd, classItem) => new { sd.schedule, sd.day, classItem })
            .Where(x => x.classItem.Weeks.Odd != null)
            .Where(r => r.schedule.Group.Title == group)
            .Select(x => new LessonInfo
            {
                Day = x.day.day,
                TeacherName = x.classItem.Weeks.Odd.Teacher.Name,
                TeacherPatronymic = x.classItem.Weeks.Odd.Teacher.Patronymic,
                TeacherSurname = x.classItem.Weeks.Odd.Teacher.Surname,
                GroupTitle = x.schedule.Group.Title,
                Subject = x.classItem.Weeks.Odd.SubjectForSite,
                EndTime = x.classItem.Class.EndTime,
                StartTime = x.classItem.Class.StartTime,
                LessonType = x.classItem.Weeks.Odd.LessonType,
                RoomName = x.classItem.Weeks.Odd.Room.Name
            }).OrderBy(x => x.StartTime)
              .ToList();
        return listResult;
    }
    public List<LessonInfo> EvenWeekScheduleOutput(string group)
    {
        var roots = GetAllTable();
        var listResult = context.Roots
            .SelectMany(r => r.Schedule)
            .SelectMany(s => s.Days, (schedule, day) => new { schedule, day })
            .SelectMany(sd => sd.day.Classes, (sd, classItem) => new { sd.schedule, sd.day, classItem })
            .Where(x => x.classItem.Weeks.Even != null)
            .Where(r => r.schedule.Group.Title == group)
            .Select(x => new LessonInfo
            {
                Day = x.day.day,
                TeacherName = x.classItem.Weeks.Even.Teacher.Name,
                TeacherPatronymic = x.classItem.Weeks.Even.Teacher.Patronymic,
                TeacherSurname = x.classItem.Weeks.Even.Teacher.Surname,
                GroupTitle = x.schedule.Group.Title,
                Subject = x.classItem.Weeks.Even.SubjectForSite,
                EndTime = x.classItem.Class.EndTime,
                StartTime = x.classItem.Class.StartTime,
                LessonType = x.classItem.Weeks.Even.LessonType,
                RoomName = x.classItem.Weeks.Even.Room.Name
            }).OrderBy(x => x.StartTime)
              .ToList();
        return listResult;
    }

    public int evenNumber(DateTime startSemester)
    {
        DateTime today = DateTime.Today;
        int count = 0;
        int obs = 0;
        double var2 = today.DayOfYear - startSemester.DayOfYear;
        for(int i = 0; i <= var2; i++)
        {
            DateTime timeNext = startSemester.AddDays(i);
            if(timeNext.DayOfWeek == DayOfWeek.Sunday)
            {
                count += 1;
                if(count > 5)
                {
                    count = 1;
                }
            }
        }
        return obs / 5;
    }



    public List<LessonInfo> SaturdayOutput(string group, DateTime startSemester, DateTime today)
    {
        int num = WeekСhecker(startSemester, today);
        int count = 0;
        int obs = 0;
        double var2 = today.DayOfYear - startSemester.DayOfYear;  
        for (int i = 0; i <= var2; i++)
        {
            DateTime timeNext = startSemester.AddDays(i);
            if (timeNext.DayOfWeek == DayOfWeek.Sunday)         //зробити це окремою ф-цією
            {
                count += 1;
                if (count > 5)
                {
                    count = 1;
                }
            }

        }
        DateTime d = DayOfWeek(count);
        obs = num / 5;
        var list = new List<LessonInfo>();
        if (obs % 2 != 0)
        {
            list = context.Roots
             .SelectMany(r => r.Schedule)
             .Where(r => r.Group.Title == group)
             .SelectMany(s => s.Days, (schedule, day) => new { schedule, day })
             .Where(d => d.day.day == ((DayOfWeek)count).ToString().ToUpper())

             .SelectMany(sd => sd.day.Classes, (sd, classItem) => new { sd.schedule, sd.day, classItem })
             .Where(x => x.classItem.Weeks.Even != null)
             .Select(x => new LessonInfo
             {
                 Day = "SATURDAY",
                 TeacherName = x.classItem.Weeks.Even.Teacher.Name,
                 TeacherPatronymic = x.classItem.Weeks.Even.Teacher.Patronymic,
                 TeacherSurname = x.classItem.Weeks.Even.Teacher.Surname,
                 GroupTitle = x.schedule.Group.Title,
                 Subject = x.classItem.Weeks.Even.SubjectForSite,
                 EndTime = x.classItem.Class.EndTime,
                 StartTime = x.classItem.Class.StartTime,
                 LessonType = x.classItem.Weeks.Even.LessonType,
                 RoomName = x.classItem.Weeks.Even.Room.Name
             }).OrderBy(x => x.StartTime).ToList();
        }
        else
        {
            list = context.Roots
             .SelectMany(r => r.Schedule)
             .Where(r => r.Group.Title == group)
             .SelectMany(s => s.Days, (schedule, day) => new { schedule, day })
             .Where(d => d.day.day == ((DayOfWeek)count).ToString().ToUpper())

             .SelectMany(sd => sd.day.Classes, (sd, classItem) => new { sd.schedule, sd.day, classItem })
             .Where(x => x.classItem.Weeks.Even != null)
             .Select(x => new LessonInfo
             {
                 Day = "SATURDAY",
                 TeacherName = x.classItem.Weeks.Odd.Teacher.Name,
                 TeacherPatronymic = x.classItem.Weeks.Odd.Teacher.Patronymic,
                 TeacherSurname = x.classItem.Weeks.Odd.Teacher.Surname,
                 GroupTitle = x.schedule.Group.Title,
                 Subject = x.classItem.Weeks.Odd.SubjectForSite,
                 EndTime = x.classItem.Class.EndTime,
                 StartTime = x.classItem.Class.StartTime,
                 LessonType = x.classItem.Weeks.Odd.LessonType,
                 RoomName = x.classItem.Weeks.Odd.Room.Name
            }).OrderBy(x => x.StartTime).ToList();
        }

        return list;
    }


    public List<LessonInfo> Group(string group)
    {
        List<LessonInfo> list = new();
        List<LessonInfo> listSaturday = new();
        //тимчасові дані
        var d1 = "17/02/2025";
        var d2 = "30/03/2025";
        DateTime startSaturday = DateTime.Parse(d1);//ці дані потрібно буде звідкись брати
        DateTime endSaturaday = DateTime.Parse(d2);
       //


        DateTime today = DateTime.Today;//DateTime.Today()

        var roots = context.Roots
            .Select(r => r.Semester).ToList();

        foreach (var root in roots) {
            DateTime StartSemester = DateTime.Parse(root.StartDay);

            int number = WeekСhecker(StartSemester, today);
            int dayWeek = WeekСhecker(startSaturday, today);
            if (number % 2 == 0)
            {
                list = EvenWeekScheduleOutput(group);
                if (today.DayOfYear < endSaturaday.DayOfYear && today.DayOfYear >= startSaturday.DayOfYear)//змінити
                {
                    listSaturday = SaturdayOutput(group, startSaturday, today);//можна перераховувати тижні і відповідно перевіряти суботи
                }
                list.AddRange(listSaturday);
            }
            else
            {
                list = OddWeekScheduleOutput(group);
                if (today.DayOfYear < endSaturaday.DayOfYear && today.DayOfYear >= startSaturday.DayOfYear) //змінити
                {
                    listSaturday = SaturdayOutput(group, startSaturday, today);
                }
                list.AddRange(listSaturday);
            }
        }
        return list;
    }

    //Конкретна дата + група
    public List<LessonInfo> SearchByDate(string group,DateTime date)
    {
        var lesson = context.Roots
           .SelectMany(r => r.Schedule)
            .Where(r => r.Group.Title == group)
            .SelectMany(s => s.Days, (schedule, day) => new { schedule, day })
            .SelectMany(sd => sd.day.Classes, (sd, classItem) => new { sd.schedule, sd.day, classItem })
            .Where(x => x.classItem.Weeks.Odd != null)
            .Where(r => r.schedule.Group.Title == group)
            .Where(d=>d.day.day == date.DayOfWeek.ToString().ToUpper())
            .Select(s => new LessonInfo
            {
                Day = s.day.day,
                TeacherName = s.classItem.Weeks.Even.Teacher.Name,
                TeacherPatronymic = s.classItem.Weeks.Even.Teacher.Patronymic,
                TeacherSurname = s.classItem.Weeks.Even.Teacher.Surname,
                GroupTitle = s.schedule.Group.Title,
                Subject = s.classItem.Weeks.Even.SubjectForSite,
                EndTime = s.classItem.Class.EndTime,
                StartTime = s.classItem.Class.StartTime,
                LessonType = s.classItem.Weeks.Even.LessonType,
                RoomName = s.classItem.Weeks.Even.Room.Name
            }).ToList();

        return lesson;
        //потрібно враховувати суботи
        //шукає потрібний день за допомогою DayOfYear так можна буде врахорвувати суботи
        //змінити сам вивід,має виводити тільки 1 день на сам сайт(це міняти в View)
        //Бажано зробити сьогодні

        //UPD:
        //Перевірка на входження в ренж навчання по суботам(якщо воно є)
        //Врахування парності тижня за допомогою ф-ції WeekCheker
    }
 
}


