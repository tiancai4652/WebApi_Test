using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MyBlazorWithWebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            //builder.Services.AddHttpClient("ServerAPI", client =>
            //{
            //    client.BaseAddress = new Uri(@"http://192.168.0.115:7788");
            //    client.DefaultRequestHeaders.Add("Accept", "application/json");
            //});

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("http://192.168.0.115:7788") });

            await builder.Build().RunAsync();
        }
    }
}
