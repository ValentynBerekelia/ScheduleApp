namespace ScheduleApp.Models
{
    public class Weeks
    {
        public int WeeksId { get; set; }
        public Lesson? Even { get; set; }
        public Lesson? Odd { get; set; }
    }
}
