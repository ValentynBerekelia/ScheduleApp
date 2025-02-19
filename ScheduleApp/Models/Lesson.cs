namespace ScheduleApp.Models
{
    public class Lesson
    {
        public int LessonId { get; set; }
        public Teacher? Teacher { get; set; }
        public string? LinkToMeeting { get; set; }
        public string? SubjectForSite { get; set; }
        public string? LessonType { get; set; }
        public Room? Room { get; set; }

        //public object? TemporarySchedule { get; set; }//тип object не підтримується
    }
}
