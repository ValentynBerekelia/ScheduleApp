namespace ScheduleApp.Models
{
    public class Root
    {
        public int RootId { get; set; }         
        
        public ICollection<ScheduleItem>? Schedule { get; set; } = null!;
        public Semester? Semester { get; set; } = null!;
    }
}
