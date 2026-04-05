using Fomo.Domain.Entities;
using Fomo.Infrastructure.Repositories;

namespace Fomo.Infrastructure.ExternalServices.MailService.Alerts
{
    public class GenericAlert : IGenericAlert
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        
        public GenericAlert (IEmailService emailService, IUserRepository userRepository)
        {
            _emailService = emailService;
            _userRepository = userRepository;
        }
        
        public async Task SendAlertAsync(AlertType alertType, string stock, string indicator)
        {
            var usersList = await _userRepository.GetUsersByAlertAsync(alertType);

            string subject = "🚀 Nueva Alerta de FOMO";

            if (usersList != null)
            {
                foreach (var user in usersList)
                {
                    string body = BuildAlertEmail(user.Name, stock, indicator);
                    await _emailService.SendAsync(user.Email, subject, body);
                }
            }
        }

        private string BuildAlertEmail(string userName, string stock, string indicator)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            </head>
            <body style='margin:0; padding:0; background-color:#f3f4f6; font-family: system-ui, Arial, sans-serif;'>

            <div style='max-width:600px; margin:40px auto; background:#ffffff; border-radius:12px; overflow:hidden; border:1px solid #e0e0e0;'>
            
                <!-- Header -->
                <div style='background:#333; padding:24px 32px; display:flex; align-items:center;'>
                    <span style='color:#ffffff; font-size:24px; font-weight:800; letter-spacing:-0.5px;'>FOMO 🚀</span>
                </div>

                <!-- Body -->
                <div style='padding:32px;'>
                    <h2 style='margin:0 0 8px 0; font-size:20px; color:#111;'>¡Hola, {userName}!</h2>
                    <p style='margin:0 0 24px 0; color:#555; font-size:15px; line-height:1.5;'>
                        Un usuario obtuvo un resultado positivo que podría interesarte.
                    </p>

                    <!-- Card -->
                    <div style='background:#f8f8f8; border:1px solid #ddd; border-radius:10px; overflow:hidden; margin-bottom:24px;'>
                        <div style='padding:12px 20px; border-bottom:1px solid #ddd;'>
                            <span style='font-size:12px; color:#555; font-weight:500; text-transform:uppercase; letter-spacing:0.05em;'>Resultado destacado</span>
                        </div>
                        <div style='padding:20px;'>
                            <table style='border-collapse:collapse; width:100%;'>
                                <tr>
                                    <td style='vertical-align:top; padding-right:20px; width:80px;'>
                                        <span style='font-size:32px; font-weight:800; color:#111;'>{stock}</span>
                                    </td>
                                    <td style='vertical-align:top;'>
                                        <div style='font-size:12px; color:#555; margin-bottom:4px;'>Indicador</div>
                                        <div style='font-size:13px; font-weight:600; color:#3b82f6; border:1px solid #3b82f6; border-radius:8px; padding:6px 12px; display:inline-block;'>
                                            {indicator}
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <p style='margin:0; color:#9ca3af; font-size:13px; line-height:1.5;'>
                        Recordá que los indicadores <strong>NO SON GARANTÍA</strong> de éxito.<br/>
                        El único propósito de este sitio es académico.
                    </p>
                </div>

                <!-- Footer -->
                <div style='background:#333; padding:16px 32px; text-align:center;'>
                    <p style='margin:0; color:#9ca3af; font-size:12px;'>
                        FOMO
                    </p>
                </div>

            </div>

        </body>
        </html>";
        }
    }
}
