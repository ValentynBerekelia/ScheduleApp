namespace ScheduleApp.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public bool? Disable { get; set; } = null!;
        public string? Title { get; set; } = null!;


        public int? ScheduleItemId { get; set; }
        public ScheduleItem? ScheduleItem { get; set; } = null!;
    }
}
