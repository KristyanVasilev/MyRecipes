namespace MyRecipes.Services.Data
{
    using MyRecipes.Web.ViewModels.Recipes;
    using System.Threading.Tasks;

    public interface IRecipesService
    {
        Task CreateAsync(CreateRecipeInputModel input);
    }
}
