using Microsoft.AspNetCore.Mvc;

namespace preference.Controllers
{
    public class IssuesController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}