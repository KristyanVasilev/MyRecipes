namespace MyRecipes.Web.ViewModels.Recipes
{
    using AutoMapper;
    using MyRecipes.Data.Models;
    using MyRecipes.Services.Mapping;

    public class EditRecipeViewModel : BaseRecipeInputModel, IMapFrom<Recipe>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Recipe, EditRecipeViewModel>()
                .ForMember(x => x.CookingTime, opt =>
                    opt.MapFrom(x => (int)x.CookingTime.TotalMinutes))
                .ForMember(x => x.PreparationTime, opt =>
                    opt.MapFrom(x => (int)x.PreparationTime.TotalMinutes));
        }
    }
}
