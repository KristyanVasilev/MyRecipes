namespace MyRecipes.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MyRecipes.Web.ViewModels.Recipes;

    public interface IRecipesService
    {
        Task CreateAsync(CreateRecipeInputModel input, string userId, string imagePath);

        IEnumerable<T> GetAll<T>(int pageNumber, int itemsPerPage = 12);

        IEnumerable<T> GetRandom<T>(int count);

        int GetRecipesCount();

        T GetSingleRecipe<T>(int id);

        Task UpdateAsync(int id, EditRecipeViewModel input);

        Task DeleteAsync(int id);

        IEnumerable<T> GetRecipeByIngredients<T>(IEnumerable<int> ingredientsIds);
    }
}
