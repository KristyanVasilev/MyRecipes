namespace MyRecipes.Web.ViewModels.SearchRecipes
{
    using System.Collections.Generic;

    public class SearchIndexViewModel
    {
        public IEnumerable<IgredientsNameIdViewModel> Ingredients { get; set; }
    }
}
