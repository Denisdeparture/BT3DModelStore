using Amazon.Runtime.Internal.Util;
using BuisnesLogic.Model;
using BuisnesLogic.ServicesInterface;
using Confluent.Kafka;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc.Formatters;
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
        private SmtpConfigurationModel _config;
        private readonly ILogger<PostSender> _logger;
    
        public PostSender(ILogger<PostSender> logger, SmtpConfigurationModel model)
        {
            _config = model ?? throw new ArgumentNullException(nameof(model));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public void SendMailSmtp(MimeMessage message, SmtpMessageModel config_message, int port = 465, bool useSsl = true)
        {
            const string protocol = "smtp";
            string connectionString = string.Format("{0}.{1}.2", protocol, config_message.provider, config_message.provider);
            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    smtpClient.Connect(connectionString, port, useSsl);
                    smtpClient.Authenticate(_config.UserName, _config.UserPassword);
                }
                catch(Exception ex)
                {
                    _logger.LogError(DateTime.UtcNow + " " + this.ToString() + Environment.NewLine + "Smtp connect get exception: " + ex.Message);
                }
                finally
                {
                    smtpClient.Dispose();
                }
                message.To.Add(new MailboxAddress(string.Empty, config_message.RecipientEmail));
                message.From.Add(new MailboxAddress(config_message.nameCompanyOrAdministration, config_message.SenderEmail));
                smtpClient.Send(message);
            }
        }
    }
}
