using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Shared;

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
    }
}
