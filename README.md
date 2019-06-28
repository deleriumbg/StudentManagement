# Student Management System

Task: 

  Create a web ASP.NET Framework MVC project called Student Management. 
The project should retrieve data from Dynamics CRM and display the following pages:

  a. Students page: first page shown in the site (home page), it should display all students depending on drop down menu filter with value of "Student Status". When a status is selected it should show result table with these fields (Student ID, Name, Show Details button, Show Cases button). The page should support pagination (number of students on each page should be 10).
  
  b. Student Details page: you can access it when you click on Show Details button in Students page and it will show the following info (First Name, Last Name, Student Status, Program, Program Advisor) about the selected student. The page should have two modes - first mode called "View mode" shows all data about the student as read only and on the bottom of the page there should be "Edit button". When clicked it will enable all fields to be changed and show "Save button". When it has been clicked you should validate all the fields, after that reflect the changes to the CRM. There should be "Back button" returning the user to Students page.
 
  c. Cases page: you can access it when you click on show Cases button in Students page and it will show grid of all cases for the selected student. The grid should contain the following columns (Subject, Case Title, Case Status, Resolve button). The Resolve button should resolve the case in the CRM. There should be "Back button" returning the user to Student page and a Search box allowing the user to search for a case by a part of the Case Title.
	
  
Notes: 

  a. All retrieved records should be active records in the CRM. 
  
  b. The connection to Dynamics CRM should be done via singleton class.
  
  c. Early Bound entity class should be used.
