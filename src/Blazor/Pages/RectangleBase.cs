using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using SharedLibrary;

namespace Blazor.Pages
{
    public class RectangleBase : ComponentBase
    {
        protected HubConnection _hubConnection;

        protected Task Send(SelectRectangle selectRectangle) => _hubConnection.SendAsync("select", selectRectangle);

        protected bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        protected override async Task OnInitializedAsync()
        {
            var configurationHandler = new ConfigurationHandler();
            var configuration = configurationHandler.Get();
            var url = $"{configuration.Backend}/api/";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();

            _hubConnection.On<SelectRectangle>("select", (selectRectangle) =>
            {
                StateHasChanged();
            });

            await _hubConnection.StartAsync();
        }
    }
}
