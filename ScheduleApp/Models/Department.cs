namespace ScheduleApp.Models
{
    public class Department
    {
        public int DepartmentId { get; set; } 
        public string? Name { get; set; } = null!;
        public bool Disable { get; set; }   
    }
}
