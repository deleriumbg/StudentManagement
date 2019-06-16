using UOPXRM;
using log4net;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using StudentManagement.Models;
using StudentManagement.Models.Enums;
using Microsoft.Xrm.Sdk;

namespace StudentManagement.DAL
{
    public class StudentDataAccess
    {
        private readonly IConnection _connection;
        private readonly IOrganizationService _service;
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public StudentDataAccess()
        {
            this._connection = CrmConnection.Instance;
            this._service = _connection.organizationService;
        }

        public List<StudentViewModel> RetriveAllActiveRecords()
        {
            try
            {
                using (var context = new XrmServiceContext(_service))
                {
                    _log.Info("Executing retrieve all active students");
                    var allActiveStudents = (from studentSet in context.AccountSet
                                             where studentSet.StateCode.Equals(0)
                                             select new StudentViewModel()
                                             {
                                                 StudentID = studentSet.New_UserID,
                                                 Name = $"{studentSet.New_FirstName} {studentSet.New_FamilyName}"
                                             })
                                             .ToList();

                    _log.Info($"Retrieved {allActiveStudents.Count()} total student records");
                    return allActiveStudents;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught while retrieving all active student records - {ex.Message}");
                return null;
            }
        }

        public List<StudentViewModel> RetriveStudentRecordsByStatus(string status)
        {
            try
            {
                if (!Enum.IsDefined(typeof(StudentStatus), int.Parse(status)))
                {
                    _log.Error($"Trying to execute retrieve all active students with not existing status {status}");
                    return null;
                }

                int studentStatus = int.Parse(status);
                using (var context = new XrmServiceContext(_service))
                {
                    _log.Info($"Executing retrieve all active students with status {(StudentStatus)studentStatus}");
                    var studentRecordsByStatus = (from studentSet in context.AccountSet
                                             where studentSet.StateCode.Equals(AccountState.Active) && studentSet.New_StudentStatus.Value.Equals(status)
                                             select new StudentViewModel()
                                             {
                                                 StudentID = studentSet.New_UserID,
                                                 Name = $"{studentSet.New_FirstName} {studentSet.New_FamilyName}"
                                             })
                                             .ToList();

                    _log.Info($"Retrieved {studentRecordsByStatus.Count()} student records with status {(StudentStatus)studentStatus}");
                    return studentRecordsByStatus;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught while retrieving All Student Records with Student Status {status} - {ex.Message}");
                return null;
            }
        }

        public DetailsViewModel RetriveStudentDetails(string studentId)
        {
            try
            {
                //Check if studentId is null
                if (string.IsNullOrWhiteSpace(studentId))
                {
                    _log.Error($"Trying to execute retrieve student details for non existing student id");
                    return null;
                }

                using (var context = new XrmServiceContext(_service))
                {
                    _log.Info($"Executing retrieve student details for student with id {studentId}");
                    DetailsViewModel details = (from student in context.AccountSet
                                                where student.New_UserID.Equals(studentId)
                                                select new DetailsViewModel()
                                                {
                                                    FirstName = student.New_FirstName,
                                                    LastName = student.New_FamilyName,
                                                    StudentStatus = (StudentStatus)student.New_StudentStatus.Value,
                                                    Program = student.new_programid != null ? student.new_programid.Name.ToString() : string.Empty,
                                                    ProgramAdvisor = student.new_ProgramAdvisorid != null ? student.new_ProgramAdvisorid.Name.ToString() : string.Empty,
                                                    StudentId = student.New_UserID
                                                })
                                                .FirstOrDefault();

                    if (details == null)
                    {
                        _log.Info($"No student found with id {studentId}");
                        return null;
                    }

                    _log.Info($"Retrieved details for student with ID {studentId}: First name {details.FirstName}, Last name {details.LastName}, Student status {details.StudentStatus}, " +
                        $"Program {details.Program}, Program advisor {details.ProgramAdvisor}");
                    return details;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught while retrieving Student Details Record for Student with ID {studentId} - {ex.Message}");
                return null;
            }
        }

        public List<ProgramAdvisorViewModel> RetrieveAllProgramAdvisors()
        {
            try
            {
                using (var context = new XrmServiceContext(_service))
                {
                    _log.Info("Executing retrieve all program advisors");
                    var advisors = (from user in context.SystemUserSet
                                    join teams in context.TeamMembershipSet on user.SystemUserId equals teams.SystemUserId
                                    join team in context.TeamSet on teams.TeamId equals team.TeamId
                                    where team.Name == "Advisors"
                                    select new ProgramAdvisorViewModel
                                    {
                                        ProgramAdvisorId = user.SystemUserId.ToString(),
                                        Name = user.FullName
                                    })
                                    .ToList();

                    _log.Info($"Retrieved {advisors.Count()} student advisors");
                    return advisors;
                }
            }
            catch (Exception)
            {
                _log.Error($"Exception caught while trying to retrieve all program advisors");
                return null;
            }
        }

        public Account UpdateStudentDetails(string studentId, DetailsViewModel model)
        {
            try
            {
                //Check if studentId is null
                if (string.IsNullOrWhiteSpace(studentId))
                {
                    _log.Error($"Trying to execute update student details for non existing student id");
                    return null;
                }

                using (var context = new XrmServiceContext(_service))
                {
                    _log.Info($"Executing update student details for student with id {studentId}");
                    var studentObject = (from student in context.AccountSet
                                         where student.New_UserID.Equals(studentId)
                                         select student)
                                         .FirstOrDefault();

                    if (studentObject == null)
                    {
                        _log.Info($"No student found with id {studentId}");
                        return null;
                    }

                    if (studentObject.New_FirstName != model.FirstName || studentObject.New_FamilyName != model.LastName 
                        || studentObject.New_StudentStatus.Value != (int)model.StudentStatus)
                    {
                        if (studentObject.New_FirstName != model.FirstName)
                        {
                            string oldFirstName = studentObject.New_FirstName;
                            studentObject.New_FirstName = model.FirstName;
                            _log.Info($"Updated the first name for student with ID {studentId} from {oldFirstName} to {model.FirstName}");
                        }

                        if (studentObject.New_FamilyName != model.LastName)
                        {
                            string oldLastName = studentObject.New_FamilyName;
                            studentObject.New_FamilyName = model.LastName;
                            _log.Info($"Updated the last name for student with ID {studentId} from {oldLastName} to {model.LastName} ");
                        }

                        if (studentObject.New_StudentStatus.Value != (int)model.StudentStatus)
                        {
                            int oldStudentStatus = studentObject.New_StudentStatus.Value;
                            studentObject.New_StudentStatus = new OptionSetValue((int)model.StudentStatus);
                            _log.Info($"Updated the student status for student with ID {studentId} from {(StudentStatus)oldStudentStatus} to {model.StudentStatus} ");
                        }

                        context.UpdateObject(studentObject);
                        context.SaveChanges();
                    }
                    else
                    {
                        _log.Info($"The new entered values are equal to the old ones");
                    }

                    return studentObject;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught while trying to update Student Details for Student with ID {studentId} - {ex.Message}");
                return null;
            }
        }
    }
}