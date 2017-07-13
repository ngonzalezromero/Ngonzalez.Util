using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ngonzalez.Util.Implementation
{
    public interface IAsyncMail
    {
        IAsyncMail SmtpHost(string host);
        IAsyncMail SmtpPort(int port);
        IAsyncMail SmtpUser(string user);
        IAsyncMail SmtpPassword(string pass);
        IAsyncMail SenderAddress(string s);
        IAsyncMail SenderName(string s);
        IAsyncMail Subject(string s);
        IAsyncMail Body(string b);
        IAsyncMail Cc(string c);
        IAsyncMail Archive(string a);
        IAsyncMail Addresses(List<string> a);
        Task<bool> BuildMailAsync();
    }
}