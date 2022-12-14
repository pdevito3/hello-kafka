namespace RecipeManagement.SharedTestHelpers.Fakes.Ingredient;

using AutoBogus;
using RecipeManagement.Domain.Ingredients;
using RecipeManagement.Domain.Ingredients.Dtos;

public sealed class FakeIngredient
{
    public static Ingredient Generate(IngredientForCreationDto ingredientForCreationDto)
    {
        return Ingredient.Create(ingredientForCreationDto);
    }

    public static Ingredient Generate()
    {
        return Generate(new FakeIngredientForCreationDto().Generate());
    }
}