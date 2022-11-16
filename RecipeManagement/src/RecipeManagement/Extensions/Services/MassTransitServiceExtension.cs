namespace RecipeManagement.Extensions.Services;

using RecipeManagement.Resources;
using MassTransit;
using SharedKernel.Messages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Confluent.Kafka;
using Domain.Recipes.Features;

public static class MassTransitServiceExtension
{
    public static void AddMassTransitServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        if (!env.IsEnvironment(Consts.Testing.IntegrationTestingEnvName) 
            && !env.IsEnvironment(Consts.Testing.FunctionalTestingEnvName))
        {
            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
                
                x.AddRider(rider =>
                {
                    rider.AddConsumer<AddToBook>();
                    rider.AddProducer<IRecipeAdded>("recipe-added");
                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<IRecipeAdded>("recipe-added", nameof(RecipeManagement), c =>
                        {
                            c.ConfigureConsumer<AddToBook>(context);
                            c.CreateIfMissing(t => t.NumPartitions = 1);
                        });
                    });
                });
            });
            services.AddOptions<MassTransitHostOptions>();
        }
    }
}
