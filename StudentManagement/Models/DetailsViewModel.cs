using StudentManagement.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Models
{
    public class DetailsViewModel
    {
        private const string RegExPattern = "^[A-Za-z ]+$";

        [Required(ErrorMessage = "Please enter the student's first name.")]
        [RegularExpression(RegExPattern, ErrorMessage = "First Name must contain only english letters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter the student's last name.")]
        [RegularExpression(RegExPattern, ErrorMessage = "Last Name must contain only english letters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EnumDataType(typeof(StudentStatus))]
        [Display(Name = "Student Status")]
        public StudentStatus StudentStatus { get; set; }

        public string Program { get; set; }

        [Display(Name = "Program Advisor")]
        public string ProgramAdvisor { get; set; }

        public string StudentId { get; set; }

        public IEnumerable<ProgramAdvisorViewModel> AllAdvisors { get; set; }
    }
}