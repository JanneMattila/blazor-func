using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazor.Pages
{
    public class FetchDataBase : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; }

        protected WeatherForecast[] _forecasts;

        protected override async Task OnInitializedAsync()
        {
            _forecasts = await Http.GetJsonAsync<WeatherForecast[]>("sample-data/weather.json");
        }

        public class WeatherForecast
        {
            public DateTime Date { get; set; }

            public int TemperatureC { get; set; }

            public string Summary { get; set; }

            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        }
    }
}
