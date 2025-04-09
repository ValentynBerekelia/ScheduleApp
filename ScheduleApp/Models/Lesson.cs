namespace ScheduleApp.Models
{
    public class Lesson
    {
        public int LessonId { get; set; } 
        public Teacher? Teacher { get; set; } = null!;
        public string? LinkToMeeting { get; set; } = null!;
        public string? SubjectForSite { get; set; } = null!;
        public string? LessonType { get; set; } = null!;
        public Room? Room { get; set; } = null!;

        //public object? TemporarySchedule { get; set; }//тип object не підтримується
    }
}
