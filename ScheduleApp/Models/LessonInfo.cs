namespace ScheduleApp.Models
{
    public class LessonInfo
    {
        public string WeekType { get; set; } = string.Empty;
        public string LessonType { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string TeacherSurname { get; set; } = string.Empty;
        public string TeacherPatronymic { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string GroupTitle { get; set; } = string.Empty;
        public string Day { get; set; } = string.Empty; 
        public string StartTime {  get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
    }
}