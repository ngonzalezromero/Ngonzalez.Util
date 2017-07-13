using System.Collections.Generic;
namespace Ngonzalez.Util.Implementation
{
    public interface IMail
    {
        void BuildMail(string smtpHost, int smtpPort, string smtpUserMail, string smtpPassword, string sender, string senderName, string subject, string body, string cc, string archive, List<string> addresses);
    }
}