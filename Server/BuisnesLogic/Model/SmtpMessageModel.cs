using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Model
{
    public record SmtpMessageModel(string nameCompanyOrAdministration,string RecipientEmail, string SenderEmail, string provider, string domain_region);
   
}
