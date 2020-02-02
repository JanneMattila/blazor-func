using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using SharedLibrary;

namespace Func
{
    public static class SignalRFunction
    {
        [FunctionName("negotiate")]
        public static SharedLibrary.ConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "funchub")] SignalRConnectionInfo connectionInfo)
        {
            // Temp workaround: https://github.com/dotnet/aspnetcore/issues/18697
            return new SharedLibrary.ConnectionInfo()
            {
                Url = connectionInfo.Url,
                AccessToken = connectionInfo.AccessToken
            };
        }

        [FunctionName("select")]
        public static Task SendMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] SelectRectangle message,
            [SignalR(HubName = "funchub")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "select",
                    Arguments = new[] { message }
                });
        }
    }
}
