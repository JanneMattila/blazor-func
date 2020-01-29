using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SharedLibrary;

namespace Blazor.Pages
{
    public class FetchDataBase : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; }

        protected WeatherForecast[] _forecasts;

        protected override async Task OnInitializedAsync()
        {
            var configurationHandler = new ConfigurationHandler();
            var configuration = configurationHandler.Get();
            var url = $"{configuration.Backend}/api/Weather";
            _forecasts = await Http.GetJsonAsync<WeatherForecast[]>(url);
        }
    }
}
