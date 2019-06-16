using UOPXRM;
using log4net;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using StudentManagement.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace StudentManagement.DAL
{
    public class CaseDataAccess
    {
        private readonly IConnection _connection;
        private readonly IOrganizationService _service;
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CaseDataAccess()
        {
            this._connection = CrmConnection.Instance;
            this._service = _connection.organizationService;
        }

        public List<CaseViewModel> RetriveStudentCases(string studentId, string search)
        {
            try
            {
                //Check if studentId is null
                if (string.IsNullOrWhiteSpace(studentId))
                {
                    _log.Error($"Trying to execute retrieve student cases for non existing student id");
                    return null;
                }

                using (var context = new XrmServiceContext(_service))
                {
                    _log.Info($"Executing retrieve student cases for student with id {studentId}");
                    var getStudentCases = (from student in context.AccountSet
                                           join cases in context.IncidentSet on student.AccountId equals cases.CustomerId.Id
                                           where student.New_UserID.Equals(studentId) && cases.Title.Contains(search)
                                           select new CaseViewModel()
                                           {
                                               Subject = cases.SubjectId != null ? cases.SubjectId.Name : string.Empty,
                                               CaseTitle = cases.Title,
                                               CaseStatus = cases.new_caseid != null ? cases.new_caseid.Name : string.Empty,
                                               CaseId = cases.IncidentId.Value,
                                               StudentId = studentId
                                           })
                                           .ToList();

                    _log.Info($"Retrieved {getStudentCases.Count()} cases for student with ID {studentId}");
                    return getStudentCases;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught while retrieving Student Cases for Student with ID {studentId} - {ex.Message}");
                return null;
            }
        }

        public void ResolveCase(Guid caseId)
        {
            try
            {
                using (var context = new XrmServiceContext(_service))
                {
                    _log.Info($"Executing resolve case for case with id {caseId}");

                    Incident selectedCase = (from incident in context.IncidentSet
                                            where incident.IncidentId.Equals(caseId)
                                            select incident)
                                            .FirstOrDefault();

                    if (selectedCase == null)
                    {
                        _log.Info($"No case found with id {caseId}");
                        return;
                    }

                    var statuses = (from status in context.New_requeststatusSet
                                         select status)
                                         .ToList();

                    //Set case status to "Completed"
                    selectedCase.new_caseid.Id = statuses[0].Id;
                    
                    //Create Incident Resolution
                    var incidentResolution = new IncidentResolution
                    {
                        Subject = "Case Resolved",
                        IncidentId = new EntityReference(Incident.EntityLogicalName, selectedCase.Id),
                        ActualEnd = DateTime.Now
                    };

                    //Close Incident
                    var closeIncidenRequst = new CloseIncidentRequest
                    {
                        IncidentResolution = incidentResolution,
                        Status = new OptionSetValue(5)
                    };

                    _service.Execute(closeIncidenRequst);

                    context.UpdateObject(selectedCase);
                    context.SaveChanges();
                    _log.Info($"Set case status to Completed for Case with ID {caseId}");
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Exception caught while trying to resolve case with ID {caseId} - {ex.Message}");
            }
        }
    }
}