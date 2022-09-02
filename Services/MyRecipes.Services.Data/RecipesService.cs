namespace MyRecipes.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using MyRecipes.Data.Common.Repositories;
    using MyRecipes.Data.Models;
    using MyRecipes.Services.Mapping;
    using MyRecipes.Web.ViewModels.Recipes;

    public class RecipesService : IRecipesService
    {
        private readonly IDeletableEntityRepository<Recipe> recipeRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;

        public RecipesService(
            IDeletableEntityRepository<Recipe> recipesRepository,
            IDeletableEntityRepository<Ingredient> ingredientsRepository)
        {
            this.recipeRepository = recipesRepository;
            this.ingredientsRepository = ingredientsRepository;
        }

        public async Task CreateAsync(CreateRecipeInputModel input, string userId, string imagePath)
        {
            var recipe = new Recipe
            {
                Name = input.Name,
                CategoryId = input.CategoryId,
                CookingTime = TimeSpan.FromMinutes(input.CookingTime),
                PreparationTime = TimeSpan.FromMinutes(input.PreparationTime),
                PortionsCount = input.PortionsCount,
                UserId = userId,
                Instructions = input.Instructions,
            };


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

            var allowedExtensions = new[] { "jpg", "png" };
            Directory.CreateDirectory($"{imagePath}/recipes/");

            foreach (var image in input.Images)
            {
                var extension = Path.GetExtension(image.FileName).TrimStart('.');

                if (!allowedExtensions.Any(x => extension.EndsWith(x)))
                {
                    throw new Exception($"Invalid image extension {extension}");
                }

                var dbImage = new Image
                {
                    UserId = userId,
                    Extension = extension,
                };

                recipe.Images.Add(dbImage);

                var physicalPath = $"{imagePath}/recipes/{dbImage.Id}.{extension}";

                using (Stream fileStream = new FileStream(physicalPath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
            }

            await this.recipeRepository.AddAsync(recipe);
            await this.recipeRepository.SaveChangesAsync();
        }

        // Formula for pagination (pageNumber - 1) * itemsPerPage
        public IEnumerable<T> GetAll<T>(int pageNumber, int itemsPerPage = 12)
            => this.recipeRepository
                   .AllAsNoTracking()
                   .OrderByDescending(x => x.Id)
                   .Skip((pageNumber - 1) * itemsPerPage)
                   .Take(itemsPerPage)
                   .To<T>()
                   .ToList();

        // Sorting recipes by random
        public IEnumerable<T> GetRandom<T>(int count)
            => this.recipeRepository.All().OrderBy(x => Guid.NewGuid())
                   .To<T>()
                   .Take(count)
                   .ToList();

        public IEnumerable<T> GetRecipeByIngredients<T>(IEnumerable<int> ingredientsIds)
        {
            var query = this.recipeRepository.All().AsQueryable();

            // Натрупване на много критерий
            foreach (var ingredientId in ingredientsIds)
            {
                query = query.Where(x => x.Ingredients.Any(i => i.IngredientId == ingredientId));
            }

            return query.To<T>().ToList();
        }

        public int GetRecipesCount()
            => this.recipeRepository.All().Count();

        public T GetSingleRecipe<T>(int id)
            => this.recipeRepository
                   .AllAsNoTracking()
                   .Where(x => x.Id == id)
                   .To<T>()
                   .FirstOrDefault();

        public async Task UpdateAsync(int id, EditRecipeViewModel input)
        {
            var recipe = await this.recipeRepository.All().FirstOrDefaultAsync(x => x.Id == id);

            recipe.Name = input.Name;
            recipe.Instructions = input.Instructions;
            recipe.CookingTime = TimeSpan.FromMinutes(input.CookingTime);
            recipe.PreparationTime = TimeSpan.FromMinutes(input.PreparationTime);
            recipe.PortionsCount = input.PortionsCount;
            recipe.CategoryId = input.CategoryId;

            await this.recipeRepository.SaveChangesAsync();
        }
    }
}
