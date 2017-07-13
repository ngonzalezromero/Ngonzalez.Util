using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Ngonzalez.Util.Implementation
{
    public sealed class AsyncMail : IAsyncMail
    {
        private string smtpHost { get; set; }
        private int smtpPort { get; set; }
        private string smtpUserMail { get; set; }
        private string smtpPassword { get; set; }
        private string senderAddress { get; set; }
        private string senderName { get; set; }
        private string subject { get; set; }
        private string body { get; set; }
        private string cc { get; set; }
        private string archive { get; set; }
        List<string> addresses { get; set; }

        public IAsyncMail SmtpHost(string host)
        {
            smtpHost = host;
            return this;
        }
        public IAsyncMail SmtpPort(int port)
        {
            smtpPort = port;
            return this;
        }
        public IAsyncMail SmtpUser(string user)
        {
            smtpUserMail = user;
            return this;
        }

        public IAsyncMail SmtpPassword(string pass)
        {
            smtpPassword = pass;
            return this;
        }
        public IAsyncMail SenderAddress(string s)
        {
            senderAddress = s;
            return this;
        }

        public IAsyncMail SenderName(string s)
        {
            senderName = s;
            return this;
        }

        public IAsyncMail Subject(string s)
        {
            subject = s;
            return this;
        }
        public IAsyncMail Body(string b)
        {
            body = b;
            return this;
        }
        public IAsyncMail Cc(string c)
        {
            cc = c;
            return this;
        }

        public IAsyncMail Archive(string a)
        {
            archive = a;
            return this;
        }

        public IAsyncMail Addresses(List<string> a)
        {
            addresses = a;
            return this;
        }

        public async Task<bool> BuildMailAsync()
        {
            SmtpClient client = null;
            var response = false;
            try
            {
                client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                client.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(smtpUserMail, smtpPassword);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderAddress));
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

                await client.SendAsync(message).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
                response = true;
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
            return response;
        }
    }
}
