using System.Net;
using System.Net.Mail;
using Domain.DTOs.Email;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task<bool> SendEmailAsync(EmailDto emailDto)
    {
        try
        {
            var mailMessage = new MailMessage()
            {
                Subject = emailDto.Subject,
                Body = emailDto.Body,
                IsBodyHtml = true,
                From = new MailAddress(configuration["SMTPConfig:SenderAddress"]!),
                To = { new MailAddress(emailDto.To) }
            };

            var client = new SmtpClient
            {
                Credentials = new NetworkCredential
                (
                    configuration["SMTPConfig:SenderAddress"]!,
                    configuration["SMTPConfig:Password"]!
                ),

                Port = int.Parse(configuration["SMTPConfig:Port"]!),
                EnableSsl = true,
                Host = configuration["SMTPConfig:Host"]!,
            };

            await client.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    internal void ExecuteAsync(CancellationToken none)
    {
        throw new NotImplementedException();
    }

}