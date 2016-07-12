using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTrackingSystem.Service.Models.FormModels
{
    public class EditUserFormViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public byte[] Photo { get; set; }
    }
}
