namespace BugTrackingSystem.Service.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Photo { get; set; }

        public UserRole Role { get; set; }
    }
}
