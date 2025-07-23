namespace Ecommerce.Models.ViewModels
{
    public class AccountForChangePassword
    {
        public string? Id { get; set; } = string.Empty;

        public string? Email { get; set; } = string.Empty;

        [Required]
        [Remote(action: "CheckPassword", controller: "User", areaName: "Admin", AdditionalFields = "Id", ErrorMessage = "Incorrect Password. Try anther Password")]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [Remote(action: "CheckNewPassword", controller: "User", areaName: "Admin", AdditionalFields = "OldPassword", ErrorMessage = "New Password cannot be the same as Old Password.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword", ErrorMessage = "Password Not Match ")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
