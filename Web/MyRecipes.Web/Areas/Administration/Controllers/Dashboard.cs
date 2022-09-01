namespace MyRecipes.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class Dashboard : AdministrationController
    {
        public IActionResult Index()
        {
            return this.View();
        }
    }
}
