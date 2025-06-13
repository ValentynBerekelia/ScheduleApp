namespace ScheduleApp.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public bool Disable { get; set; }
        public string? Name { get; set; } = null!;
        public string? Surname { get; set; } = null!;
        public string? Patronymic { get; set; } = null!;
        public string? Position { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public Department? Department { get; set; } = null!;

    }
}
