namespace MyRecipes.Services.Data
{
    using MyRecipes.Services.Data.ModelsDtos;

    public interface IGetCountsService
    {
        // 1. IndexViewModel();  We can use view model
        // 2. Create Dtos -> view Model
        // 3. Tuples(, , ,)
        CountsDto GetCounts();
    }
}
