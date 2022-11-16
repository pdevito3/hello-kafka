namespace RecipeManagement.Domain.Recipes.Features;

using MapsterMapper;
using MassTransit;
using SharedKernel.Messages;
using System.Threading.Tasks;
using RecipeManagement.Databases;
using Serilog;

public sealed class AddToBook : IConsumer<IRecipeAdded>
{
    public Task Consume(ConsumeContext<IRecipeAdded> context)
    {
        Log.Warning("In the consumer");
        Log.Warning("Recipe added: {@Recipe}", context.Message);
        
        Log.Warning("Recipe added to book: {Recipe}", context.Message);

        return Task.CompletedTask;
    }
}