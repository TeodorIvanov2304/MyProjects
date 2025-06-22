using Microsoft.AspNetCore.Mvc;

namespace SimpleTaskManagerApp.Controllers
{
	public class ErrorController : Controller
	{
		[Route("Error/404")]	
		public IActionResult NotFoundPage() 
		{
			Response.StatusCode = 404;
			return View("NotFound");
		}

		[Route("Error/403")]
		public IActionResult Forbidden()
		{
			Response.StatusCode = 403;

			return View("Forbidden");
		}

		[Route("Error/Error")]
		public IActionResult GeneralError()
		{
			Response.StatusCode = 500;
			return View("Error");
		}
	}
}
