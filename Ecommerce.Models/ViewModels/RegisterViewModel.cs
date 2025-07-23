
namespace Ecommerce.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Remote(action: "IsUserNameInUse", controller: "Account", areaName: "Authentication", ErrorMessage = "Username is already Used ,Try different User Name")]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = null!;

        [Remote(action: "IsEmailInUse", controller: "Account", areaName: "Authentication", ErrorMessage = "Email is already Used ,Try different Email")]
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="Password Must be Bigger than 6 char")]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password Not Match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;

        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(01[0125])[0-9]{8}$",ErrorMessage ="Invalid Phone Number")]
        public string PhoneNumber { get; set; } = null!;
        public string? Role { get; set; }
        public IEnumerable<SelectListItem> ?RolesList { get; set; } = new List<SelectListItem>();
    }

}
