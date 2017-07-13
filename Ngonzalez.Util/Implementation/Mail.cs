using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Ngonzalez.Util.Implementation
{
    public sealed class Mail : IMail
    {
        public void BuildMail(string smtpHost, int smtpPort, string smtpUserMail, string smtpPassword, string senderMail, string senderName, string subject, string body, string cc, string archive, List<string> addresses)
        {

            SmtpClient client = null;

            try
            {
                client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                client.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(smtpUserMail, smtpPassword);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderMail));
                message.Subject = subject;

                var builder = new BodyBuilder();

                if (archive != null)
                {
                    builder.Attachments.Add(archive);
                }
                builder.HtmlBody = body;
                message.Body = builder.ToMessageBody();

                if (!string.IsNullOrWhiteSpace(cc))
                {
                    message.Cc.Add(new MailboxAddress("", cc));
                }

                foreach (var item in addresses)
                {
                    message.To.Add(new MailboxAddress("", item));
                }

                client.Send(message);
                client.Disconnect(true);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }
            finally
            {
                client?.Dispose();
            }
        }
    }
}


