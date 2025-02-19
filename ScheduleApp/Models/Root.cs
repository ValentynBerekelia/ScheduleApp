namespace ScheduleApp.Models
{
    public class Root
    {
        public int RootId { get; set; }         
        
        public ICollection<ScheduleItem>? Schedule { get; set; }
        public Semester? Semester { get; set; }
    }
}
