using Microsoft.AspNetCore.Mvc;

namespace WhiteLagoon.Web.Controllers
{
    public class DashboardController : Controller
    {
        public DashboardController() { }

        public IActionResult Index()
        {
            return View("Index");
        }

        
    }
}
 