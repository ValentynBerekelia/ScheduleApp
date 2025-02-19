namespace ScheduleApp.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public bool? Disable { get; set; }
        public string? Title { get; set; }


        public int? ScheduleItemId { get; set; }
        public ScheduleItem? ScheduleItem {get;set;}
    }
}
