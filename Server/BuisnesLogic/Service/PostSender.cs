using Amazon.Runtime.Internal.Util;
using BuisnesLogic.Model;
using BuisnesLogic.ServicesInterface;
using Confluent.Kafka;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Service
{
    public class PostSender : ISender
    {
        private IConfiguration _config;
        private readonly ILogger<PostSender> _logger;
    
        public PostSender(ILogger<PostSender> logger, IConfiguration configuration)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public void SendMailSmtp(MimeMessage message, SmtpMessageModel config_message)
        {
            const string protocol = "smtp";
            string connectionString = string.Format("{0}.{1}.{2}", protocol, config_message.provider, config_message.domain_region);
            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    smtpClient.Connect(connectionString, config_message.port,config_message.useSsl);
                    smtpClient.Authenticate(_config[$"SmtpClients:{config_message.provider}:User"], _config[$"SmtpClients:{config_message.provider}:Pass"]);
                    message.To.Add(new MailboxAddress(string.Empty, config_message.RecipientEmail));
                    message.From.Add(new MailboxAddress(config_message.nameCompanyOrAdministration, config_message.SenderEmail));
                    smtpClient.Send(message);
                }
                catch(Exception ex)
                {
                    _logger.LogError(DateTime.UtcNow + " " + this.ToString() + Environment.NewLine + "Smtp connect get exception: " + ex.Message);
                }
                finally
                {
                    smtpClient.Dispose();
                }
            }
        }
    }
}
