namespace ScheduleApp.Models
{
    public class Day
    {
        public int DayId { get; set; }
        public string day { get; set; } = null!;
        public ICollection<ClassItem>? Classes { get; set; } = null!;
    }

}
