using log4net;
using StudentManagement.DAL;
using StudentManagement.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace StudentManagement.Controllers
{
    public class DetailsController : Controller
    {
        private readonly StudentDataAccess _studentDataAccess;
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DetailsController()
        {
            this._studentDataAccess = new StudentDataAccess();
        }

        // GET: /Details/Index
        public ActionResult Index(string id)
        {
            try
            {
                DetailsViewModel studentDetails = _studentDataAccess.RetriveStudentDetails(id);
                List<ProgramAdvisorViewModel> advisors = _studentDataAccess.RetrieveAllProgramAdvisors();

                if (studentDetails == null)
                {
                    _log.Error($"Retrieve student details for student id {id} returned no data.");
                    return View("Error");
                }
                if (advisors == null)
                {
                    _log.Error("Retrieve all student advisors returned no data");
                }

                studentDetails.AllAdvisors = advisors;
                _log.Info($"Successfuly retrieved student details for student {id} on GET: Details/Index/{id}");
                return View(studentDetails);
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught in the DetailsController on GET: Details/Index/{id} - {ex.Message}");
                return View("Error");
            }
        }

        // POST: /Details/Edit
        [HttpPost]
        public ActionResult Edit(string id, DetailsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var account = _studentDataAccess.UpdateStudentDetails(id, model);
                    if (account == null)
                    {
                        _log.Error($"Update student details for student id {id} returned no data");
                        return View("Error");
                    }

                    _log.Info($"Successfuly updated details for student id {id} with no validation errors");
                    return RedirectToAction($"Index/{id}");
                }
                else
                {
                    _log.Info($"Validation errors in the user input for student id {id}'s first/last name");
                    return RedirectToAction($"Index/{id}");
                }
                
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught in the DetailsController on POST: Details/Edit/{id} - {ex.Message}");
                return View("Error");
            }
        }
    }
}
