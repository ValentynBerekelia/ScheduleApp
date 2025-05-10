namespace ScheduleApp.Models
{
    public class ScheduleItem
    {
        public int ScheduleItemId { get; set; }
        public Group? Group { get; set; } = null!; 
        public ICollection<Day>? Days { get; set; } = null!;

    }
}
