namespace ASC.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(String email, string subject, string message);
    }
}
