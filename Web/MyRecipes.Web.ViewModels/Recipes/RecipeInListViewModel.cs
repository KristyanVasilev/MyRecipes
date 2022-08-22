namespace MyRecipes.Web.ViewModels.Recipes
{
    using AutoMapper;
    using MyRecipes.Data.Models;
    using MyRecipes.Services.Mapping;
    using System.Linq;

    public class RecipeInListViewModel : IMapFrom<Recipe>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Recipe, RecipeInListViewModel>()
                .ForMember(x => x.ImageUrl, opt =>
                opt.MapFrom(
                    x =>
                "/images/recipes" + x.Images.FirstOrDefault().Id + "." + x.Images.FirstOrDefault().Extension
                ));
        }
    }
}
