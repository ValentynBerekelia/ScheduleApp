using ScheduleApp.Data;
using ScheduleApp.Models;

namespace ScheduleApp.Services
{
    public static class AddingData
    {
        public static void Initialize(ApplicationContext context, JSONDeserializer scheduleService)
        {
            if (!context.Roots.Any())
            {
                var scheduleData = scheduleService.GetScheduleData().Result;

                if (scheduleData != null)
                {
                    context.Add(scheduleData);
                    context.SaveChanges();
                }
            }
            else
            {
            }
        }
    }
}
