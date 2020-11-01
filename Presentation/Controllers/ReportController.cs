using Logic.Services.Report;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Controllers
{
	public class ReportController : BaseController
	{
		private readonly ReportService service = new ReportService();

		// GET: Disadvs
		public ActionResult Index()
		{

			ViewData["States"] = service.States.Select(p => new SelectListItem()
			{
				Value = p.StateId.ToString(),
				Text = p.StateName,
				Selected = (p.StateName.Equals("Victoria", StringComparison.CurrentCultureIgnoreCase))
			})
			.ToList();
            ViewBag.ShowAllData = ShowAllData;
            return View(service.GetDisadges(null, ShowAllData));
		}

		public ActionResult ReportGrid(string stateId, bool showAllData = false)
        {
            ShowAllData = showAllData;
            ViewBag.ShowAllData = ShowAllData;
            int id = stateId != null ? int.Parse(stateId) : -1;
			return PartialView(service.GetDisadges(id, ShowAllData));
		}
	}
}
