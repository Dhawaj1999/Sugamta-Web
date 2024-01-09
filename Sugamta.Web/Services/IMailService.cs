using Sugamta.Web.Models;

namespace Sugamta.Web.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(Mailrequest mailrequest);
    }
}
