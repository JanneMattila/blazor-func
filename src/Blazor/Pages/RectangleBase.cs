using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using SharedLibrary;

namespace Blazor.Pages
{
    public class RectangleBase : ComponentBase
    {
        [Inject] private HttpClient Http { get; set; }

        protected HubConnection _hubConnection;

        protected Task Send(SelectRectangle selectRectangle) => _hubConnection.SendAsync("select", selectRectangle);

        protected bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        protected override async Task OnInitializedAsync()
        {
            var configurationHandler = new ConfigurationHandler();
            var configuration = configurationHandler.Get();
            //var url = $"{configuration.Backend}/api/";
            var url = $"{configuration.Backend}/api/negotiate";

            // Temp workaround: https://github.com/dotnet/aspnetcore/issues/18697
            var response = await Http.PostJsonAsync<SharedLibrary.ConnectionInfo>(url, null);
            Console.WriteLine(response.Url);
            Console.WriteLine(response.AccessToken);
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{response.Url}&access_token={response.AccessToken}")
                .Build();

            _hubConnection.On<SelectRectangle>("select", (selectRectangle) =>
            {
                StateHasChanged();
            });

            await _hubConnection.StartAsync();
        }
    }
}
