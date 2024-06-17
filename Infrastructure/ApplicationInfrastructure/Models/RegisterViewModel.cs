using System.ComponentModel.DataAnnotations;

namespace WebClient.Models
{
    public class RegisterViewModel
    {
        [Display(Name = "Ваше Имя")]
        public string? Name { get; set; }
        [Display(Name = "Ваша Фамилия")]
        public string? LastName { get; set; }
        [Display(Name = "Ваш Email")]
        public string Mail { get; set; } = null!;
        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
    }
}
