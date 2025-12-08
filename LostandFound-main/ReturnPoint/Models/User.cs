namespace ReturnPoint.Models
{
    public class User
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        // used by other code (GetLoggedInUser uses "Name")
        public string Name { get; set; }
        public string GradeSection { get; set; }
    }
}