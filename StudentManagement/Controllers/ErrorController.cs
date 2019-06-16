using log4net;
using System.Reflection;
using System.Web.Mvc;

namespace StudentManagement.Controllers
{
    public class ErrorController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Error
        public ActionResult Error()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            _log.Error("User tryed to access invalid URL");
            return View();
        }
    }
}