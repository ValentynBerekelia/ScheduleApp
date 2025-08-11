namespace ScheduleApp.Models
{
    public class SemesterClass
    {
        public int SemesterClassId { get; set; }
        public string? StartTime { get; set; } = null!;
        public string? EndTime { get; set; } = null!;
        public string? ClassName { get; set; }//null
        //public Semester Semesters { get; set; }
    }
}
