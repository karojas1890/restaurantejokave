using System.Net.Mail;
using System.Net;

namespace WebApplication1.Service
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendCode(string email, string code)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:SenderPassword"]
                ),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:SenderEmail"], "JOKAVE"),
                Subject = "Código de Verificación - JOKAVE",
                IsBodyHtml = true,
                Body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f5f5f5;'>
                <div style='background-color: white; padding: 20px; border-radius: 8px; text-align: center; max-width: 500px; margin: auto;'>
                    <img src='https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRkhgWjPfzlIYYd-HWnml33c-AYJHPm7RnfAg&s' alt='Logo JOKAVE' style='max-width: 150px; margin-bottom: 20px;' />
                    <h2 style='color: #333;'>Código de Verificación</h2>
                    <p style='font-size: 16px; color: #555;'>Gracias por preferirnos. Tu código de verificación es:</p>
                    <p style='font-size: 32px; font-weight: bold; color: #007bff; margin: 20px 0;'>{code}</p>
                    
                </div>
            </div>"
            };

            mailMessage.To.Add(email);
            smtpClient.Send(mailMessage);
        }

    }
}
