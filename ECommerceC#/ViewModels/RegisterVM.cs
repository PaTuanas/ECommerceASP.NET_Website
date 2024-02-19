using System.ComponentModel.DataAnnotations;

namespace ECommerceC_.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage ="*")]
        [MaxLength(20, ErrorMessage = "Please enter a maximum of 20 characters")]
        public string UserId { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Please enter a maximum of 50 characters")]
        public string FullName { get; set; }    
        public bool Gender { get; set; }
        public bool Role { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth {  get; set; }

        [MaxLength(60, ErrorMessage = "Please enter a maximum of 60 characters")]
		[Display(Name = "Address")]
		public string Address { get; set; }

        [MaxLength(24, ErrorMessage = "Please enter a maximum of 24 characters")]
        [RegularExpression(@"0[98753]\d{8}", ErrorMessage ="Incorrect phone number format")]
		[Display(Name = "Phone number")]
		public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Incorrect email format")]
		[Display(Name = "Email")]
		public string Email { get; set; }
        public string? UserImg {  get; set; }
    }
}
