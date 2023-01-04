using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;


[assembly: FunctionsStartup(typeof(FunctionAppTimerTrigger.Startup))]
namespace FunctionAppTimerTrigger
{


    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddLogging();  
            builder.Services.AddSingleton<IServerRepository, ServerRepository>();
        }
    }
}
