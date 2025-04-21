using Azure;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Serialization;
using ScheduleApp.Data;
using ScheduleApp.Models;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
using System.Web.WebPages;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class ScheduleService
{
    private readonly ApplicationContext context;

    public ScheduleService(ApplicationContext context)
    {
        this.context = context;
    }

    public List<TeacherInfo> TeacherList()
    {
        var teachers = context.Roots
            .SelectMany(r => r.Schedule)
            .SelectMany(s => s.Days)
            .SelectMany(d => d.Classes)
            .Where(c => c.Weeks != null && c.Weeks.Even.Teacher != null)
            .Select(c => new TeacherInfo
            {
                TeacherSurname = c.Weeks.Even.Teacher.Surname,
                TeacherName = c.Weeks.Even.Teacher.Name,
                TeacherPatronymic = c.Weeks.Even.Teacher.Patronymic
            })
            .Distinct()
            .ToList();
        var group = context.Roots
            .SelectMany(r => r.Schedule)
            .Where(r => r.Group != null)
            .Select(s => new TeacherInfo
            {
                GroupName = s.Group.Title
            })
            .Distinct()
            .ToList();
        teachers.AddRange(group);
        return teachers;
    }


    //ScheduleController
    public List<LessonInfo> SearchForTeacher(string surname) 
    { 
        DateTime startSemester = DateTime.ParseExact(context.Roots.Select(r => r.Semester.StartDay).First(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
        bool isEven = OddsOfWeek(startSemester, DateTime.Today) % 2 == 0;
            var lesson = (from root in context.Roots
                      from schedule in root.Schedule
                      from day in schedule.Days
                      from classItem in day.Classes
                      let teacher = classItem.Weeks.Even.Teacher
                      where teacher.Surname + " " + teacher.Name + " " + teacher.Patronymic == surname
                      let week = isEven ? classItem.Weeks.Even : classItem.Weeks.Odd
                      group new { schedule, day, classItem } by new
                      {
                          teacher.Name,
                          teacher.Surname,
                          teacher.Patronymic,
                          day.day,
                          week.SubjectForSite,
                          classItem.Class.StartTime,
                          classItem.Class.EndTime,
                          week.LessonType,
                          RoomName = week.Room.Name
                      } into g
                      orderby g.Key.StartTime
                      select new LessonInfo
                      {
                          WeekType = isEven ? "Even" : "Odd",
                          Day = g.Key.day,
                          TeacherName = g.Key.Name,
                          TeacherPatronymic = g.Key.Patronymic,
                          TeacherSurname = g.Key.Surname,
                          GroupTitle = string.Join(", ", g.Select(x => x.schedule.Group.Title)),
                          Subject = g.Key.SubjectForSite,
                          StartTime = g.Key.StartTime,
                          EndTime = g.Key.EndTime,
                          LessonType = g.Key.LessonType,
                          RoomName = g.Key.RoomName
                      }).OrderBy(s => s.StartTime).ToList();
        return lesson;
    }

    //public static int WeekСheсker(DateTime Start, DateTime Today)
    //{
    //    int count = 0;//1!
    //    double var2 = Today.DayOfYear - Start.DayOfYear;
    //    for (int i = 0; i < var2; i++)
    //    {
    //        DateTime timeNext = Start.AddDays(i);
    //        if (timeNext.DayOfWeek == DayOfWeek.Sunday)
    //        {
    //            count += 1;
    //        }
    //    }
    //    return count;
    //}
    public static int OddsOfWeek(DateTime Start, DateTime Today)
    {
        int count = 1;//0!
        double var2 = Today.DayOfYear - Start.DayOfYear;
        for (int i = 0; i < var2; i++)
        {
            DateTime timeNext = Start.AddDays(i);
            if (timeNext.DayOfWeek == DayOfWeek.Saturday)
            {
                count += 1;
            }
        }
        return count;
    }
    public List<LessonInfo> WeekScheduleOutput(string group,bool isEven)
    {
        var listResult = (from root in context.Roots
                          from schedule in root.Schedule
                          from day in schedule.Days
                          from classItem in day.Classes
                          where classItem.Weeks.Odd != null
                          where schedule.Group.Title == @group
                          let week = isEven ? classItem.Weeks.Even : classItem.Weeks.Odd
                          select new LessonInfo
                          {
                              WeekType = isEven ? "Even" : "Odd",
                              Day = day.day,
                              TeacherName = week.Teacher.Name,
                              TeacherPatronymic = week.Teacher.Patronymic,
                              TeacherSurname = week.Teacher.Surname,
                              GroupTitle = schedule.Group.Title,
                              Subject = week.SubjectForSite,
                              EndTime = classItem.Class.EndTime,
                              StartTime = classItem.Class.StartTime,
                              LessonType = week.LessonType,
                              RoomName = week.Room.Name
                          }).OrderBy(s => s.StartTime).ToList();
        return listResult;
    }


    //змінити
    public List<LessonInfo> SaturdayOutput(string group, DateTime startSemester, DateTime today)
    {
        int count = 1;
        double var2 = today.DayOfYear - startSemester.DayOfYear;
        for (int i = 0; i <= var2; i++)
        {
            DateTime timeNext = startSemester.AddDays(i);
            if (timeNext.DayOfWeek == DayOfWeek.Sunday)
            {
                count += 1;
                if (count > 5)
                {
                    count = 1;
                }
            }

        }

        var saturdayRange = context.SaturdayClasses.FirstOrDefault();
        if (saturdayRange == null)
        {
            return new List<LessonInfo> { new LessonInfo { Day = "" } };
        }
        string FirstWeek = saturdayRange.WeekType;
        string SecondWeek = saturdayRange.SecondWeekType;


        int totalDays = (today - saturdayRange.StartSaturday).Days;
        int totalWeek = totalDays / 7;
        //якщо більше 7 то second
        //проблема з днями(reverse view)
        bool weekType = totalWeek <= 5;// ? FirstWeek : SecondWeek;
        string dayName = ((DayOfWeek)count).ToString().ToUpper();
        string currentWeekType = weekType ? FirstWeek : SecondWeek;
        var lesson = (from root in context.Roots
                      from schedule in root.Schedule
                      where schedule.Group.Title == @group
                      from day in schedule.Days
                      where day.day == dayName
                      from classItem in day.Classes
                      let week = currentWeekType == "Even" ? classItem.Weeks.Even : classItem.Weeks.Odd
                      select new LessonInfo
                      {
                          WeekType = currentWeekType,//weekType ? FirstWeek : SecondWeek,
                          Day = "SATURDAY",
                          TeacherName = week.Teacher.Name,
                          TeacherPatronymic = week.Teacher.Patronymic,
                          TeacherSurname = week.Teacher.Surname,
                          GroupTitle = schedule.Group.Title,
                          Subject = week.SubjectForSite,
                          EndTime = classItem.Class.EndTime,
                          StartTime = classItem.Class.StartTime,
                          LessonType = week.LessonType,
                          RoomName = week.Room.Name
                      }).OrderBy(x => x.StartTime).ToList();

        return lesson;
    }

    public List<LessonInfo> SearchByGroup(string group)
    {
        List<LessonInfo> list = new List<LessonInfo>();
        List<LessonInfo> listSaturday = new List<LessonInfo>();
        var saturdayRange = context.SaturdayClasses.FirstOrDefault();
        DateTime startSaturday = saturdayRange.StartSaturday;
        DateTime endSaturday = saturdayRange.EndSaturday;
        DateTime startSemester = DateTime.ParseExact(context.Roots.Select(r => r.Semester.StartDay).First(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

        bool isEven = OddsOfWeek(startSemester, DateTime.Today) % 2 == 0;
        list = WeekScheduleOutput(group, isEven);
        if (DateTime.Today >= startSaturday && DateTime.Today <= endSaturday)
        {
            listSaturday = SaturdayOutput(group, startSaturday, DateTime.Today);
        }
        list.AddRange(listSaturday);
        return list;
    }

    //оптимізувати цю функцію
    public List<LessonInfo> SearchByDate(string group, DateTime date)
    {
        var saturdayRange = context.SaturdayClasses.FirstOrDefault();
        if (saturdayRange == null)
        {
            return new List<LessonInfo> { new LessonInfo { Day = "" } };
        }
        DateTime startSaturday = saturdayRange.StartSaturday;
        DateTime endSaturday = saturdayRange.EndSaturday;
        DateTime startSemester = DateTime.ParseExact(context.Roots.Select(r => r.Semester.StartDay).First(),"dd/MM/yyyy",CultureInfo.InvariantCulture);
        if ((date.DayOfWeek == DayOfWeek.Saturday && (date.DayOfYear >= endSaturday.DayOfYear || date.DayOfYear <= startSaturday.DayOfYear))|| date.DayOfWeek == DayOfWeek.Sunday)
        {
            return new List<LessonInfo> { new LessonInfo { Day = "" } };
        }
        if (date.DayOfWeek == DayOfWeek.Saturday)
        {
            return SaturdayOutput(group,startSaturday,date);
        }
        bool isEven = OddsOfWeek(startSemester, date) % 2 == 0;
        var lesson = (from root in context.Roots
                      from schedule in root.Schedule
                      where schedule.Group.Title == @group
                      from day in schedule.Days
                      where day.day == date.DayOfWeek.ToString().ToUpper()
                      from classItem in day.Classes
                      let week = isEven ? classItem.Weeks.Even : classItem.Weeks.Odd
                      select new LessonInfo
                      {
                          WeekType = isEven ? "Even" : "Odd",
                          Day = day.day,
                          TeacherName = week.Teacher.Name,
                          TeacherPatronymic = week.Teacher.Patronymic,
                          TeacherSurname = week.Teacher.Surname,
                          GroupTitle = schedule.Group.Title,
                          Subject = week.SubjectForSite,
                          EndTime = classItem.Class.EndTime,
                          StartTime = classItem.Class.StartTime,
                          LessonType = week.LessonType,
                          RoomName = week.Room.Name
                      }).OrderBy(s => s.StartTime).ToList();
        return lesson;
    }



    //new function teacher and group

    public List<LessonInfo> TeacherAndDate(string surname, DateTime date)
    {
        var saturdayRange = context.SaturdayClasses.FirstOrDefault();
        if (saturdayRange == null)
        {
            return new List<LessonInfo> { new LessonInfo { Day = "" } };
        }
        DateTime startSaturday = saturdayRange.StartSaturday;
        DateTime endSaturday = saturdayRange.EndSaturday;
        DateTime startSemester = DateTime.ParseExact(context.Roots.Select(r => r.Semester.StartDay).First(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
        if ((date.DayOfWeek == DayOfWeek.Saturday && (date.DayOfYear >= endSaturday.DayOfYear || date.DayOfYear <= startSaturday.DayOfYear)) || date.DayOfWeek == DayOfWeek.Sunday)
        {
            return new List<LessonInfo> { new LessonInfo { Day = "" } };
        }
        if (date.DayOfWeek == DayOfWeek.Saturday)
        {
            return SaturdayTeacher(surname, startSaturday, date);
        }
        bool isEven = OddsOfWeek(startSemester, date) % 2 == 0;

        var lesson = (from root in context.Roots
                  from schedule in root.Schedule
                  from day in schedule.Days
                  where day.day == date.DayOfWeek.ToString().ToUpper()
                  from classItem in day.Classes
                  let teacher = classItem.Weeks.Even.Teacher
                  where day.day == date.DayOfWeek.ToString().ToUpper()
                  where teacher.Surname + " " + teacher.Name + " " + teacher.Patronymic == surname
                  let week = isEven ? classItem.Weeks.Even : classItem.Weeks.Odd
                  group new { schedule, day, classItem } by new
                  {
                      teacher.Name,
                      teacher.Surname,
                      teacher.Patronymic,
                      day.day,
                      week.SubjectForSite,
                      classItem.Class.StartTime,
                      classItem.Class.EndTime,
                      week.LessonType,
                      RoomName = week.Room.Name
                  } into g
                  orderby g.Key.StartTime
                  select new LessonInfo
                  {
                      WeekType = isEven ? "Even" : "Odd",
                      Day = g.Key.day,
                      TeacherName = g.Key.Name,
                      TeacherPatronymic = g.Key.Patronymic,
                      TeacherSurname = g.Key.Surname,
                      GroupTitle = string.Join(", ", g.Select(x => x.schedule.Group.Title)),
                      Subject = g.Key.SubjectForSite,
                      StartTime = g.Key.StartTime,
                      EndTime = g.Key.EndTime,
                      LessonType = g.Key.LessonType,
                      RoomName = g.Key.RoomName
                  }).OrderBy(s => s.StartTime).ToList();
        return lesson;
    }
    public List<LessonInfo> SaturdayTeacher(string surname, DateTime startSemester, DateTime today)
    {
        int count = 1;
        double var2 = today.DayOfYear - startSemester.DayOfYear;
        for (int i = 0; i <= var2; i++)
        {
            DateTime timeNext = startSemester.AddDays(i);
            if (timeNext.DayOfWeek == DayOfWeek.Sunday)
            {
                count += 1;
                if (count > 5)
                {
                    count = 1;
                }
            }

        }

        var saturdayRange = context.SaturdayClasses.FirstOrDefault();
        if (saturdayRange == null)
        {
            return new List<LessonInfo> { new LessonInfo { Day = "" } };
        }
        string FirstWeek = saturdayRange.WeekType;
        string SecondWeek = saturdayRange.SecondWeekType;


        int totalDays = (today - saturdayRange.StartSaturday).Days;
        int totalWeek = totalDays / 7;
        //якщо більше 7 то second
        //проблема з днями(reverse view)
        bool weekType = totalWeek <= 5;// ? FirstWeek : SecondWeek;
        string dayName = ((DayOfWeek)count).ToString().ToUpper();
        string currentWeekType = weekType ? FirstWeek : SecondWeek;
        var lesson = (from root in context.Roots
                      from schedule in root.Schedule
                      from day in schedule.Days
                      where day.day == dayName
                      from classItem in day.Classes
                      let week = currentWeekType == "Even" ? classItem.Weeks.Even : classItem.Weeks.Odd
                      where week.Teacher.Surname + " " + week.Teacher.Name + " " + week.Teacher.Patronymic == surname
                      select new LessonInfo
                      {
                          WeekType = currentWeekType,//weekType ? FirstWeek : SecondWeek,
                          Day = "SATURDAY",
                          TeacherName = week.Teacher.Name,
                          TeacherPatronymic = week.Teacher.Patronymic,
                          TeacherSurname = week.Teacher.Surname,
                          GroupTitle = schedule.Group.Title,
                          Subject = week.SubjectForSite,
                          EndTime = classItem.Class.EndTime,
                          StartTime = classItem.Class.StartTime,
                          LessonType = week.LessonType,
                          RoomName = week.Room.Name
                      }).OrderBy(x => x.StartTime).ToList();

        return lesson;
    }
}


