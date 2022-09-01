namespace MyRecipes.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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

        public IEnumerable<T> GetAll<T>(int pageNumber, int itemsPerPage = 12)
        {
            var recipes = this.recipeRepository.AllAsNoTracking()
                 .OrderByDescending(x => x.Id)
                 .Skip((pageNumber - 1) * itemsPerPage)
                 .Take(itemsPerPage)
                 .To<T>()
                 .ToList();

            // Formula for pagination (pageNumber - 1) * itemsPerPage
            return recipes;
        }

        // Sorting recipes by random
        public IEnumerable<T> GetRandom<T>(int count)
            => this.recipeRepository.All().OrderBy(x => Guid.NewGuid())
                .To<T>()
                .Take(count)
                .ToList();

        public int GetRecipesCount()
            => this.recipeRepository.All().Count();

        public T GetSingleRecipe<T>(int id)
            => this.recipeRepository
                   .AllAsNoTracking()
                   .Where(x => x.Id == id)
                   .To<T>()
                   .FirstOrDefault();
    }
}
