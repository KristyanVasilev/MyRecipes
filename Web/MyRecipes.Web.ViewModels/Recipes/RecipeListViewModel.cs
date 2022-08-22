namespace MyRecipes.Web.ViewModels.Recipes
{
    using System.Collections.Generic;

    public class RecipeListViewModel : PagingViewModel
    {
        public IEnumerable<RecipeInListViewModel> Recipes { get; set; }
    }
}
