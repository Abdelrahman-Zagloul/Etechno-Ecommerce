
namespace Ecommerce.Models.ViewModels
{
    public class AccountViewModel
    {
        public string? Id { get; set; }
        [Remote(action: "IsUserNameInUse", controller: "Account", AdditionalFields = "Id", ErrorMessage = "Username is already in use ,Try different User Name")]
        public string? UserName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public string Role { get; set; }
        public bool isLocked { get; set; }
    }
}
