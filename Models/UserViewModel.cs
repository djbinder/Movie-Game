using System.ComponentModel.DataAnnotations;

namespace movieGame.Models {
    public class RegisterViewModel {
        [Required]
        [Display (Name = "First Name")]
        public string UserFirstName { get; set; }

        [Required]
        [Display (Name = "Last Name")]
        public string UserLastName { get; set; }

        [Required]
        [EmailAddress]
        [RegularExpression ("^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$", ErrorMessage = "Please enter a valid email address")]
        [Display (Name = "Email")]
        public string UserEmail { get; set; }

        [Required]
        [MinLength (8, ErrorMessage = "Password must be at least 8 characters long!")]
        [DataType (DataType.Password)]
        [Display (Name = "Password")]
        public string UserPassword { get; set; }

        [Required]
        [Compare ("UserPassword", ErrorMessage = "Passwords must match!")]
        [DataType (DataType.Password)]
        [Display (Name = "Confirm Password")]
        public string Confirm { get; set; }
    }

    public class LoginViewModel {
        [Required]
        [EmailAddress]
        // [RegularExpression ("^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$", ErrorMessage = "Please enter a valid email address")]
        [Display (Name = "Email")]
        public string UserEmail { get; set; }

        [Required]
        [DataType (DataType.Password)]
        [Display (Name = "Password")]
        public string UserPassword { get; set; }

    }

    public class UserViewModel {
        public RegisterViewModel RegisterViewModel { get; set; }
        public LoginViewModel LoginViewModel { get; set; }
    }
}