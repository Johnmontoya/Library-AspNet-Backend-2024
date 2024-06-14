using Backend.Dtos;
using Backend.Interfaces;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;

namespace Backend.Core
{
    public class MailService : IMailService
    {
        private readonly MailSettingDto _mailSettings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mailSettings"></param>
        public MailService(IOptions<MailSettingDto> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        /// <summary>
        /// Configuracion de la plantilla de email y la información con la que se envia
        /// </summary>
        /// <param name="htmlMailDto"></param>
        /// <returns></returns>
        public bool SendHTMLMail(HTMLMailDto htmlMailDto, string templateUse)
        {
            try
            {
                using (MimeMessage emailMessage = new MimeMessage())
                {
                    MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                    emailMessage.From.Add(emailFrom);

                    MailboxAddress emailTo = new MailboxAddress(htmlMailDto.EmailToName, htmlMailDto.EmailToId);
                    emailMessage.To.Add(emailTo);
                    string filePath;

                    switch (templateUse)
                    {
                        case "resetPassword":
                            emailMessage.Subject = "Cambio de contraseña";
                            filePath = Directory.GetCurrentDirectory() + "\\Templates\\EmailResetPassword.html";
                            break;

                        case "activarCuenta":
                            emailMessage.Subject = "Activar cuenta";
                            filePath = Directory.GetCurrentDirectory() + "\\Templates\\ActivarCuenta.html";
                            break;

                        default:
                            emailMessage.Subject = "Confirmar Email";
                            filePath = Directory.GetCurrentDirectory() + "\\Templates\\EmailConfirmed.html";
                            break;
                    }

                    /*if (templateUse == "resetPassword")
                    {
                        emailMessage.Subject = "Cambio de contraseña";
                        filePath = Directory.GetCurrentDirectory() + "\\Templates\\EmailResetPassword.html";
                    }
                    else if (templateUse == "activarCuenta")
                    {
                        emailMessage.Subject = "Activar cuenta";
                        filePath = Directory.GetCurrentDirectory() + "\\Templates\\ActivarCuenta.html";
                    } else
                    {
                        emailMessage.Subject = "Confirmar Email";
                        filePath = Directory.GetCurrentDirectory() + "\\Templates\\EmailConfirmed.html";
                    }*/

                    string emailTemplateText = File.ReadAllText(filePath);

                    emailTemplateText = string.Format(emailTemplateText,
                        htmlMailDto.EmailToName,
                        DateTime.Today.Date.ToShortDateString(),
                        htmlMailDto.EmailToId,
                        htmlMailDto.ResetLink);

                    BodyBuilder emailBodyBuilder = new BodyBuilder
                    {
                        HtmlBody = emailTemplateText
                    };

                    emailMessage.Body = emailBodyBuilder.ToMessageBody();

                    using (SmtpClient smtpClient = new SmtpClient())
                    {
                        smtpClient.Connect(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                        smtpClient.Authenticate(_mailSettings.UserName, _mailSettings.Password);
                        smtpClient.Send(emailMessage);
                        smtpClient.Disconnect(true);
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
