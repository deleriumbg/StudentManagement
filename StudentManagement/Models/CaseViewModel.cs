using System;

namespace StudentManagement.Models
{
    public class CaseViewModel
    {
        public string Subject { get; set; }

        public string CaseTitle { get; set; }

        public string CaseStatus { get; set; }

        public Guid CaseId { get; set; }

        public string StudentId { get; set; }
    }
}