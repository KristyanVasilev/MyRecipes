namespace MyRecipes.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MyRecipes.Data.Common.Repositories;
    using MyRecipes.Data.Models;
    using MyRecipes.Services.Mapping;
    using MyRecipes.Web.ViewModels.Recipes;

    public class RecipesService : IRecipesService
    {
        private readonly IDeletableEntityRepository<Recipe> recipeRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;

        public RecipesService(IDeletableEntityRepository<Recipe> recipesRepository, IDeletableEntityRepository<Ingredient> ingredientsRepository)
        {
            this.recipeRepository = recipesRepository;
            this.ingredientsRepository = ingredientsRepository;
        }

        public async Task CreateAsync(CreateRecipeInputModel input, string userId)
        {
            var recipe = new Recipe
            {
                Name = input.Name,
                CategoryId = input.CategoryId,
                CookingTime = TimeSpan.FromMinutes(input.CookingTime),
                PreparationTime = TimeSpan.FromMinutes(input.PreparationTime),
                PortionsCount = input.PortionsCount,
                UserId = userId,
            };

            if (input.Ingredients != null)
            {
                foreach (var currIngredient in input.Ingredients)
                {
                    var ingredient = this.ingredientsRepository.All().FirstOrDefault(x => x.Name == currIngredient.Name);
                    if (ingredient == null)
                    {
                        ingredient = new Ingredient
                        {
                            Name = currIngredient.Name,
                        };
                    }

                    recipe.Ingredients.Add(new RecipeIngredient
                    {
                        Ingredient = ingredient,
                        Quantity = currIngredient.Quantity,
                    });
                }
            }

            await this.recipeRepository.AddAsync(recipe);
            await this.recipeRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetAll<T>(int pageNumber, int itemsPerPage = 12)
        {
           var recipes = this.recipeRepository.AllAsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();
            // formula for pagination (pageNumber - 1) * itemsPerPage

           return recipes;
        }

        public int GetRecipesCount()
            => this.recipeRepository.All().Count();
    }
}
