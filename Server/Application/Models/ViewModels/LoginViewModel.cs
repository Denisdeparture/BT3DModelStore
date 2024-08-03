using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
namespace Application.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Почта")]
        [Compare("Mail", ErrorMessage = "Неверная почта")]
        public string? Mail { get; set; }
        [Required]
        [Display(Name = "Пароль")]
        [Compare("Password", ErrorMessage = "Неверный пароль")]
        public string? Password { get; set; }
        public string? ReturnUrl { get; set; }
        // 1 yandex, 2 google
        public IList<string> IconUrlLink { get; set; } = new List<string>() { "../assets/yandex-icon.png", "../assets/google-icon.png" };
        public IEnumerable<AuthenticationScheme>? genericDatas { get; set; }

    }
}
