using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleApp.Models
{
    public class Semester
    {
        public int SemesterId { get; set; }
        public string? Description { get; set; } = null!;
        public int? Year { get; set; } = null!;
        public string? StartDay { get; set; } = null!;
        public string? EndDay { get; set; } = null!;
        public bool CurrentSemester { get; set; } 
        public bool DefaultSemester { get; set; }
        public bool Disable { get; set; }
        public ICollection<string>? SemesterDays { get; set; }
        public ICollection<SemesterClass>? SemesterClasses { get; set; }
        //public string StartSaturday {  get; set; }


        [ForeignKey("Root")]
        public int RootId { get; set; }
        public Root Root { get; set; } = null!;
    }
}
