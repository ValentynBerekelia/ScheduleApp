using System.ComponentModel.DataAnnotations;

namespace ScheduleApp.Models
{
    public class SaturdayClass
    {
        [Key]
        public int SaturdayId { get; set; }
        public DateTime StartSaturday { get; set; }
        public DateTime EndSaturday { get; set; }
        public string WeekType { get; set; } = null!;
        public string SecondWeekType { get; set; } = null!;
    }
}
