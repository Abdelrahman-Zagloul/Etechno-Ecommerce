namespace Ecommerce.Models.ViewModels
{
    public class UpdateAccountViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "User Name")]
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public List<SelectListItem>? RoleList { get; set; }
    }
}
