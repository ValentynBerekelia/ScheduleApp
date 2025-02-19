namespace ScheduleApp.Models
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public bool Disable { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? Position { get; set; }
        public string? Email { get; set; }
        public Department? Department { get; set; }

    }
}
