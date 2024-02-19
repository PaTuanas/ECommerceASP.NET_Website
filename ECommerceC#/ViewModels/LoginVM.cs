using System.ComponentModel.DataAnnotations;

namespace ECommerceC_.ViewModels
{
	public class LoginVM
	{
		[Display(Name = "User Name")]
		[Required(ErrorMessage = "Username is required")]
		public string UserName { get; set; }

		[Display(Name = "Password")]
		[Required(ErrorMessage = "Password is required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
