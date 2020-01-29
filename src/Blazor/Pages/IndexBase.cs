using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SharedLibrary;

namespace Blazor.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var configuration = await Http.GetJsonAsync<ConfigurationData>("/configuration.json");
            var configurationHandler = new ConfigurationHandler();
            configurationHandler.Set(configuration);
        }
    }
}
