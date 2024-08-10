using BuisnesLogic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ConstStorage
{
    public class SmtpConfigs
    {
        private string email;
        public SmtpConfigs(string email)
        {
            if(string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(email);
            this.email = email ?? throw new ArgumentNullException(nameof(email));
        }
        public static SmtpConfigs GetConfigs(string email) => new SmtpConfigs(email);
        public SmtpMessageModel YANDEX => new SmtpMessageModel("BT", email, "ilimbaevAshitaDenisLD@yandex.ru", "yandex", "ru" );
        public SmtpMessageModel Mail => new SmtpMessageModel("BT", email, "ilimbaevAshitaDenisLD@yandex.ru", "mail", "ru");
        public SmtpMessageModel GMAIL => new SmtpMessageModel("BT", email, "denis11you777602@gmail.com", "google", "com", 25);
    }
}
