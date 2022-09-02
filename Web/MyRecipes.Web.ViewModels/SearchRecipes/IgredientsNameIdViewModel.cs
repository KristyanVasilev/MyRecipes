namespace MyRecipes.Web.ViewModels.SearchRecipes
{
    using MyRecipes.Data.Models;
    using MyRecipes.Services.Mapping;

    public class IgredientsNameIdViewModel : IMapFrom<Ingredient>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}