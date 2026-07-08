using Microsoft.AspNetCore.Mvc;

namespace RavenFinance.Controllers
{
    public class FinanceRecordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
