using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Stravaig.FamilyTree.Blazor.Services;
using Stravaig.FamilyTreeGenerator.Requests.Handlers.Services;

namespace Stravaig.FamilyTree.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });
            builder.Services.AddSingleton<IRelationshipRenderer, EnglishLanguageRelationshipRenderer>();
            builder.Services.AddTransient<IWebFootnoteOrganiser, WebFootnoteOrganiser>();

            await builder.Build().RunAsync();
        }
    }
}
