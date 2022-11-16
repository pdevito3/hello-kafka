namespace RecipeManagement.Domain.Recipes.Features;

using SharedKernel.Messages;
using MapsterMapper;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Dtos;
using RecipeManagement.Databases;
using Serilog;

public static class AddRecipeProducer
{
    public sealed class Command : IRequest<bool>
    {
        public RecipeForCreationDto Recipe;

        public Command(RecipeForCreationDto recipe)
        {
            Recipe = recipe;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly ITopicProducer<IRecipeAdded> _publishEndpoint;
        private readonly IMapper _mapper;
        private readonly RecipesDbContext _db;

        public Handler(RecipesDbContext db, IMapper mapper, ITopicProducer<IRecipeAdded> publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
            _db = db;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            Log.Logger.Warning("AddRecipeProducer: {@Recipe}", request.Recipe);
            
            var message = new RecipeAdded
            {
                Title = request.Recipe.Title
            };
            
            // produce message with kafka 
            await _publishEndpoint.Produce(
                message,
                // new Message<Null, IRecipeAdded> { Value = message }, 
                cancellationToken);

            return true;
        }
    }
}