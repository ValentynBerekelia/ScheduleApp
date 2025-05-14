namespace ScheduleApp.Models
{
    public class ClassItem
    {
        public int ClassItemID { get; set; }
        //public Day? days { get; set; }
        public Weeks? Weeks { get; set; } = null!;
        public SemesterClass? Class { get; set; } = null!;
    }
}
