using BuisnesLogic.Model;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface
{
    public interface ISender
    {
        public void SendMailSmtp(MimeMessage message, SmtpMessageModel config_message);
    }
}
