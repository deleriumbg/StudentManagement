using log4net;
using PagedList;
using StudentManagement.DAL;
using StudentManagement.Models;
using StudentManagement.Models.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace StudentManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentDataAccess _studentDataAccess;
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HomeController()
        {
            this._studentDataAccess = new StudentDataAccess();
        }

        // GET: /Home/Index
        public ActionResult Index(int? page)
        {
            try
            {
                List<StudentViewModel> students = _studentDataAccess.RetriveAllActiveRecords();

                if (students == null)
                {
                    _log.Error("Retrieve all active records in the HomeController returned zero results.");
                    return View("Error");
                }

                _log.Info($"Successfuly retrieved page {page} on Home/Index");
                return View(students.ToPagedList(page ?? 1, 10));
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught in the HomeController on GET: /Home/Index page {page} - {ex.Message}");
                return View("Error");
            }
        }

        // GET: /Home/Status/{status}
        public ActionResult Status(string status, int? page)
        {
            try
            {
                List<StudentViewModel> filteredStudents = _studentDataAccess.RetriveStudentRecordsByStatus(status);

                if (!Enum.IsDefined(typeof(StudentStatus), int.Parse(status)))
                {
                    _log.Error($"Trying to execute retrieve all active students with not existing status {status}");
                    return View("Error");
                }

                if (filteredStudents == null)
                {
                    _log.Error($"Retrieve all active records with status {(StudentStatus)int.Parse(status)} in the HomeController returned zero results.");
                    return View("Error");
                }

                _log.Info($"Successfuly retrieved page {page} on Home/Status/{status}");
                return View(filteredStudents.ToPagedList(page ?? 1, 10));
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught in the HomeController on GET: /Home/Status/{status} page {page} - {ex.Message}");
                return View("Error");
            }
        }
    }
}