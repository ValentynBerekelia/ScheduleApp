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
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.Xml;
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
            .Where(r=>r.Group != null)
            .Select(s=>new TeacherInfo
            {
                GroupName = s.Group.Title
            })
            .Distinct()
            .ToList();
        teachers.AddRange(group);
        return teachers;
    }


    //ScheduleController
    public List<LessonInfo> SearchForTeacher(string surname) //зробити окрему View для даної ф-ції
    {
        List<LessonInfo> lesson = new List<LessonInfo>();
        var Start = context.Roots.Select(r => r.Semester.StartDay).ToList();
        DateTime startSemester = DateTime.ParseExact(Start[0].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        int num = OddsOfWeek(startSemester, DateTime.Today);
        if (num % 2 == 0)
        {
            lesson = (from root in context.Roots
                      from schedule in root.Schedule
                      from day in schedule.Days
                      from classItem in day.Classes
                      let teacher = classItem.Weeks.Even.Teacher
                      where teacher.Surname + " " + teacher.Name + " " + teacher.Patronymic == surname
                      group new { schedule, day, classItem } by new
                      {
                          teacher.Name,
                          teacher.Surname,
                          teacher.Patronymic,
                          day.day,
                          classItem.Weeks.Even.SubjectForSite,
                          classItem.Class.StartTime,
                          classItem.Class.EndTime,
                          classItem.Weeks.Even.LessonType,
                          RoomName = classItem.Weeks.Even.Room.Name
                      } into g
                      orderby g.Key.StartTime
                      select new LessonInfo
                      {
                          WeekType = "Even",
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
        }
        else
        {
            lesson = (from root in context.Roots
                      from schedule in root.Schedule
                      from day in schedule.Days
                      from classItem in day.Classes
                      let teacher = classItem.Weeks.Odd.Teacher
                      where teacher.Surname + " " + teacher.Name + " " + teacher.Patronymic == surname
                      group new { schedule, day, classItem } by new
                      {
                          teacher.Name,
                          teacher.Surname,
                          teacher.Patronymic,
                          day.day,
                          classItem.Weeks.Odd.SubjectForSite,
                          classItem.Class.StartTime,
                          classItem.Class.EndTime,
                          classItem.Weeks.Odd.LessonType,
                          RoomName = classItem.Weeks.Odd.Room.Name
                      } into g
                      orderby g.Key.StartTime
                      select new LessonInfo
                      {
                          WeekType = "Odd",
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
        }
            return lesson;
    }

    //GroupController
    public static int WeekСhecker(DateTime Start, DateTime Today)
    {
        int count = 0;//1!
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
    public List<LessonInfo> OddWeekScheduleOutput(string group)
    {
        var listResult = (from root in context.Roots
                         from schedule in root.Schedule
                         from day in schedule.Days
                         from classItem in day.Classes
                         where classItem.Weeks.Odd != null
                         where schedule.Group.Title == @group
                         select new LessonInfo
                         {
                             WeekType = "Odd",
                             Day = day.day,
                             TeacherName = classItem.Weeks.Odd.Teacher.Name,
                             TeacherPatronymic = classItem.Weeks.Odd.Teacher.Patronymic,
                             TeacherSurname = classItem.Weeks.Odd.Teacher.Surname,
                             GroupTitle = schedule.Group.Title,
                             Subject = classItem.Weeks.Odd.SubjectForSite,
                             EndTime = classItem.Class.EndTime,
                             StartTime = classItem.Class.StartTime,
                             LessonType = classItem.Weeks.Odd.LessonType,
                             RoomName = classItem.Weeks.Odd.Room.Name
                         }).OrderBy(s=>s.StartTime).ToList();
        return listResult;
    }
    public List<LessonInfo> EvenWeekScheduleOutput(string group)
    {
        var listResult = (from root in context.Roots
                          from schedule in root.Schedule
                          from day in schedule.Days
                          from classItem in day.Classes
                          where classItem.Weeks.Even != null
                          where schedule.Group.Title == @group
                          select new LessonInfo
                          {
                              WeekType = "Even",
                              Day = day.day,
                              TeacherName = classItem.Weeks.Even.Teacher.Name,
                              TeacherPatronymic = classItem.Weeks.Even.Teacher.Patronymic,
                              TeacherSurname = classItem.Weeks.Even.Teacher.Surname,
                              GroupTitle = schedule.Group.Title,
                              Subject = classItem.Weeks.Even.SubjectForSite,
                              EndTime = classItem.Class.EndTime,
                              StartTime = classItem.Class.StartTime,
                              LessonType = classItem.Weeks.Even.LessonType,
                              RoomName = classItem.Weeks.Even.Room.Name
                          }).OrderBy(s => s.StartTime).ToList();
        return listResult;
    }



    public List<LessonInfo> SaturdayOutput(string group, DateTime startSemester, DateTime today)
    {
        int num = OddsOfWeek(startSemester, today);
        int count = 1;
        int obs = 0;
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
        string Week = "";
        string SecondWeek = "";
        foreach (var sat in context.SaturdayClasses)
        {
            Week = sat.WeekType;
            SecondWeek = sat.SecondWeekType;
        }
        var s = ((DayOfWeek)count).ToString();
        var res = startSemester.DayOfWeek.ToString();
        obs = num / 5;
        var list = new List<LessonInfo>();
        //(obs % 2 != 0) - 1 тиждень
        //(obs % 2) == 0) - 2 тиждень
        if((obs % 2 != 0) && Week == "Even" || (obs % 2) == 0 && SecondWeek == "Even")
        {
            list = (from root in context.Roots
                    from schedule in root.Schedule
                    where schedule.Group.Title == @group
                    from day in schedule.Days
                    where day.day == ((DayOfWeek)count).ToString().ToUpper()
                    from classItem in day.Classes
                    select new LessonInfo
                    {
                        WeekType = "Even",
                        Day = "SATURDAY",
                        TeacherName = classItem.Weeks.Even.Teacher.Name,
                        TeacherPatronymic = classItem.Weeks.Even.Teacher.Patronymic,
                        TeacherSurname = classItem.Weeks.Even.Teacher.Surname,
                        GroupTitle = schedule.Group.Title,
                        Subject = classItem.Weeks.Even.SubjectForSite,
                        EndTime = classItem.Class.EndTime,
                        StartTime = classItem.Class.StartTime,
                        LessonType = classItem.Weeks.Even.LessonType,
                        RoomName = classItem.Weeks.Even.Room.Name
                    }).OrderBy(x => x.StartTime).ToList();
        }
        else if((obs % 2) == 0 && SecondWeek == "Odd" || ((obs % 2 != 0) && Week == "Odd"))
        {
            list = (from root in context.Roots
                    from schedule in root.Schedule
                    where schedule.Group.Title == @group
                    from day in schedule.Days
                    where day.day == ((DayOfWeek)count).ToString().ToUpper()
                    from classItem in day.Classes
                    select new LessonInfo
                    {
                        WeekType = "Odd",
                        Day = "SATURDAY",
                        TeacherName = classItem.Weeks.Odd.Teacher.Name,
                        TeacherPatronymic = classItem.Weeks.Odd.Teacher.Patronymic,
                        TeacherSurname = classItem.Weeks.Odd.Teacher.Surname,
                        GroupTitle = schedule.Group.Title,
                        Subject = classItem.Weeks.Odd.SubjectForSite,
                        EndTime = classItem.Class.EndTime,
                        StartTime = classItem.Class.StartTime,
                        LessonType = classItem.Weeks.Odd.LessonType,
                        RoomName = classItem.Weeks.Odd.Room.Name
                    }).OrderBy(s => s.StartTime).ToList();
        }
        return list;
    }


    public List<LessonInfo> Group(string group)
    {
        List<LessonInfo> list = new();
        List<LessonInfo> listSaturday = new();
        DateTime startSaturday = new();
        DateTime endSaturaday = new();
        foreach (var sat in context.SaturdayClasses)
        {
            startSaturday = sat.StartSaturday;
            endSaturaday = sat.EndSaturday;

        }
        DateTime today = DateTime.Today;

        var roots = context.Roots
            .Select(r => r.Semester).ToList();

        foreach (var root in roots) {
            DateTime StartSemester = DateTime.ParseExact(root.StartDay,"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            int number = OddsOfWeek(StartSemester, today);
            int dayWeek = WeekСhecker(startSaturday, today);
            if (number % 2 == 0)
            {
                list = EvenWeekScheduleOutput(group);
                if (today >= startSaturday && today <= endSaturaday)
                {
                    listSaturday = SaturdayOutput(group, startSaturday, today);
                }
                list.AddRange(listSaturday);
            }
            else
            {
                list = OddWeekScheduleOutput(group);
                if (today >= startSaturday && today <= endSaturaday)
                {
                    listSaturday = SaturdayOutput(group, startSaturday, today);
                }
                list.AddRange(listSaturday);
            }
        }
        return list;
    }

    public List<LessonInfo> SearchByDate(string group,DateTime date)
    {
        DateTime startSaturday = new();
        DateTime endSaturaday = new();
        foreach (var sat in context.SaturdayClasses)
        {
            startSaturday = sat.StartSaturday;
            endSaturaday = sat.EndSaturday;
        }

        var Start = context.Roots.Select(r => r.Semester.StartDay).ToList();
        DateTime startSemester = DateTime.ParseExact(Start[0].ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        int num = OddsOfWeek(startSemester, date);
        List<LessonInfo> lesson = new();
        if (date.DayOfWeek == DayOfWeek.Saturday)
        {
            return SaturdayOutput(group, startSaturday, date);
        }
        else
        {
            if (num % 2 == 0)
            {
                lesson = (from root in context.Roots
                         from schedule in root.Schedule
                         where schedule.Group.Title == @group
                         from day in schedule.Days
                         where day.day == date.DayOfWeek.ToString().ToUpper()
                         from classItem in day.Classes
                         select new LessonInfo
                         {
                             WeekType = "Even",
                             Day = day.day,
                             TeacherName = classItem.Weeks.Even.Teacher.Name,
                             TeacherPatronymic = classItem.Weeks.Even.Teacher.Patronymic,
                             TeacherSurname = classItem.Weeks.Even.Teacher.Surname,
                             GroupTitle = schedule.Group.Title,
                             Subject = classItem.Weeks.Even.SubjectForSite,
                             EndTime = classItem.Class.EndTime,
                             StartTime = classItem.Class.StartTime,
                             LessonType = classItem.Weeks.Even.LessonType,
                             RoomName = classItem.Weeks.Even.Room.Name
                         }).OrderBy(s => s.StartTime).ToList();
            }
            else
            {
                lesson = (from root in context.Roots
                          from schedule in root.Schedule
                          where schedule.Group.Title == @group
                          from day in schedule.Days
                          where day.day == date.DayOfWeek.ToString().ToUpper()
                          from classItem in day.Classes
                          select new LessonInfo
                          {
                              WeekType = "Odd",
                              Day = day.day,
                              TeacherName = classItem.Weeks.Odd.Teacher.Name,
                              TeacherPatronymic = classItem.Weeks.Odd.Teacher.Patronymic,
                              TeacherSurname = classItem.Weeks.Odd.Teacher.Surname,
                              GroupTitle = schedule.Group.Title,
                              Subject = classItem.Weeks.Odd.SubjectForSite,
                              EndTime = classItem.Class.EndTime,
                              StartTime = classItem.Class.StartTime,
                              LessonType = classItem.Weeks.Odd.LessonType,
                              RoomName = classItem.Weeks.Odd.Room.Name
                          }).OrderBy(s => s.StartTime).ToList();
            }
        }
         return lesson;
    }
    
}


