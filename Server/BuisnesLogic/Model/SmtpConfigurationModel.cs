using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Model
{
    public class SmtpConfigurationModel
    {
        public string UserName { get; init; } 
        public string UserPassword { get; init; }
        public SmtpConfigurationModel(string smtpClientName, string smtpClientPassword)
        {
            if (string.IsNullOrWhiteSpace(smtpClientPassword) | string.IsNullOrWhiteSpace(smtpClientName)) throw new NullReferenceException();
            UserName = smtpClientName;
            UserPassword = smtpClientPassword;
        }
    }
}
