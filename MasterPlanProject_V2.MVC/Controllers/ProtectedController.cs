using Microsoft.AspNetCore.Mvc;

namespace MasterPlanProject_V2.MVC.Controllers
{
	public class ProtectedController : Controller
	{
		[Authorize]
		public IActionResult Index()
		{
			return View();
		}
	}
}
