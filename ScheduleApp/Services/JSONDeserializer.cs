using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ScheduleApp.Models;

public class JSONDeserializer
{
    private readonly HttpClient _httpClient;

    public JSONDeserializer(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Root> GetScheduleData()
    {
        string url = "http://fmi-schedule.chnu.edu.ua/schedules/full/semester?semesterId=57";//вивести в appsettings.json
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        string jsonString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Root>(jsonString);
    }

}