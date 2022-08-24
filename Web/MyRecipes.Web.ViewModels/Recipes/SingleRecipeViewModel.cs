namespace MyRecipes.Web.ViewModels.Recipes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using MyRecipes.Data.Models;
    using MyRecipes.Services.Mapping;

    public class SingleRecipeViewModel : IMapFrom<Recipe>, IHaveCustomMappings
    {
        public string Name { get; set; }

        public string CategoryName { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UserUserName { get; set; }

        public string Image { get; set; }

        public string Instructions { get; set; }

        public TimeSpan PreparationTime { get; set; }

        public TimeSpan CookingTime { get; set; }

        public int PortionsCount { get; set; }

        public IEnumerable<IngredientsViewModel> Ingredients { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Recipe, SingleRecipeViewModel>()
                .ForMember(x => x.Image, opt =>
                    opt.MapFrom(x =>
                        x.Images.FirstOrDefault().RemoteImageUrl != null ?
                        x.Images.FirstOrDefault().RemoteImageUrl :
                        "/images/recipes/" + x.Images.FirstOrDefault().Id + "." + x.Images.FirstOrDefault().Extension));
        }
    }
}
