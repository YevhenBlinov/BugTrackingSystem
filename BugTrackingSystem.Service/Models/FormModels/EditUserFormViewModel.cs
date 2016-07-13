namespace BugTrackingSystem.Service.Models.FormModels
{
    public class EditUserFormViewModel
    {
        public int UserId { get; set; }

        public bool IsPhotoEdited { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public byte[] Photo { get; set; }
    }
}
