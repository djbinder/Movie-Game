using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserFirstName {get;set;}

        [Required]
        public string UserLastName{get;set;}

        [Required]
        [EmailAddress]
        [RegularExpression("^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$", ErrorMessage="Please enter a valid email address")]
        public string UserEmail{get;set;}

        [Required]
        [MinLength(8, ErrorMessage="Password must be at least 8 characters long!")]
        [DataType(DataType.Password)]
        public string UserPassword {get;set;}

        [Required]
        [Compare("UserPassword", ErrorMessage="Passwords must match!")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [RegularExpression("^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$", ErrorMessage="Please enter a valid email address")]
        public string UserEmail {get;set;}

        [Required]
        [DataType(DataType.Password)]
        public string UserPassword {get;set;}

    }

    public class UserViewModel
    {
        public RegisterViewModel RegisterViewModel {get;set;}
        public LoginViewModel LoginViewModel {get;set;}
    }
}