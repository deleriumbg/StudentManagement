using log4net;
using StudentManagement.DAL;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace StudentManagement.Controllers
{
    public class CasesController : Controller
    {
        private readonly CaseDataAccess _caseDataAccess;
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CasesController()
        {
            this._caseDataAccess = new CaseDataAccess();
        }

        // GET: /Cases/Index/{studentId}
        public ActionResult Index(string id, string search)
        {
            try
            {
                List<CaseViewModel> cases = _caseDataAccess.RetriveStudentCases(id, search);

                if (cases.Count > 0)
                {
                    _log.Info($"Successfuly retrieved {cases.Count} cases for student id {id} on GET: Cases/Index/{id} with search query {search}");
                }
                else
                {
                    _log.Info($"Retrieve student cases for student id {id} on Cases/Index/{id} on GET: Cases/Index/{id} with search query {search} returned zero results.");
                }
                
                return View(cases);
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught in the CasesController on GET: /Cases/Index/{id} - {ex.Message}");
                return View("Error");
            }
        }

        // POST: /Cases/Resolve
        [HttpPost]
        public ActionResult Resolve(Guid caseId, string studentId)
        {
            try
            {
                //Check if studentId is null
                if (string.IsNullOrWhiteSpace(studentId))
                {
                    _log.Error($"Trying to execute resolve student case for non existing student id");
                    return View("Error");
                }

                _caseDataAccess.ResolveCase(caseId);
                _log.Info($"Successfuly resolved case with {caseId} for student {studentId} on Cases/Resolve");
                return RedirectToAction($"Index/{studentId}");
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught in the CasesController on POST: /Cases/Resolve - {ex.Message}");
                return View("Error");
            }
        }
    }
}
