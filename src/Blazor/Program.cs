using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Hosting;

namespace Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var host = builder.Build();
            await host.RunAsync();
        }
    }
}
