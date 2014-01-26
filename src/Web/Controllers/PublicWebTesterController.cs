using System.Web.Mvc;

namespace Dimensional.TinyReturns.Web.Controllers
{
    public class PublicWebTesterController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}