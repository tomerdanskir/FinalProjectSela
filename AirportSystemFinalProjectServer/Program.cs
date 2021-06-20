using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirportSystemFinalProjectServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.WaitForShutdownAsync();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        //public static async Task WaitForShutdownAsync(this IHost host)
        //{
        //    // Get the lifetime object from the DI container
        //    var applicationLifetime = host.Services.GetService<IHostApplicationLifetime>();

        //    // Create a new TaskCompletionSource called waitForStop
        //    var waitForStop = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

        //    // Register a callback with the ApplicationStopping cancellation token
        //    object p = applicationLifetime.ApplicationStopped.Register(obj =>
        //    {
        //        var tcs = (TaskCompletionSource<object>)obj;

        //        //PUT YOUR CODE HERE 

        //        // When the application stopping event is fired, set 
        //        // the result for the waitForStop task, completing it
        //        tcs.TrySetResult(null);
        //    }, waitForStop);

        //    // Await the Task. This will block until ApplicationStopping is triggered,
        //    // and TrySetResult(null) is called
        //    await waitForStop.Task;

        //    // We're shutting down, so call StopAsync on IHost
        //    await host.StopAsync();
        //}
    }
}
