using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsumerTesting
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //await CreateHostBuilder(args).Build().RunAsync();
            using (var host = CreateHostBuilder(args).Build())
            {
                await host.StartAsync();
                var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

                // do work here / get your work service ...
                Console.WriteLine("Preparing demo");
                //IPublishEndpoint _publishEndpoint = host.Services.GetRequiredService<IPublishEndpoint>();
                IBus bus = host.Services.GetRequiredService<IBus>();
                var state = new MyState() {  CorrelationId = Guid.NewGuid(), OrderId = Guid.NewGuid(), CurrentState = "Placed" };
                var cmd = new MyCommand<MyState>();
                await bus.Publish(cmd);

                Console.ReadLine();

                lifetime.StopApplication();
                await host.WaitForShutdownAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();

                        // By default, sagas are in-memory, but should be changed to a durable
                        // saga repository.
                        x.SetInMemorySagaRepositoryProvider();

                        var entryAssembly = Assembly.GetEntryAssembly();

                        x.AddConsumers(entryAssembly);
                        x.AddSagaStateMachines(entryAssembly);
                        x.AddSagas(entryAssembly);
                        x.AddActivities(entryAssembly);

                        x.UsingInMemory((context, cfg) =>
                        {
                            cfg.ConfigureEndpoints(context);
                        });
                    });

                    //services.AddSingleton<IPublishEndpoint>();
                });
    }
}
