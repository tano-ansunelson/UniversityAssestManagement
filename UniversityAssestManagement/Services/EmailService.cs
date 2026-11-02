using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using UniversityAssetsManagement.Models;
using static System.Net.Mime.MediaTypeNames;

namespace UniversityAssetsManagement.Services
{
    public class EmailService : IEmailService
    {

        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task SendAssignmentEmailAsync(AssignmentEmailModel model)
        {
          var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(model.TechnicianName, model.TechnicianEmail));
            message.Subject = $"[UAM] New Maintenance Assignment - {model.TicketId}";


            message.Body = new TextPart("html")
            {
                Text = BuildEmailBody(model)
            };


            using var client = new SmtpClient();

            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(_settings.UserName, _settings.Password);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        private static string BuildEmailBody(AssignmentEmailModel model)
        {
            return "<!DOCTYPE html>" +
                   "<html>" +
                   "<head>" +
                   "<meta charset='utf-8'/>" +
                   "<style>" +
                   "body { font-family: Arial, sans-serif; background: #f5f0e8; margin: 0; padding: 24px; }" +
                   ".card { background: #fff; border-radius: 12px; max-width: 560px; margin: 0 auto; overflow: hidden; }" +
                   ".header { background: #0e1525; padding: 28px 32px; }" +
                   ".header h1 { color: #c9a84c; font-size: 20px; margin: 0; }" +
                   ".body { padding: 28px 32px; }" +
                   ".footer { background: #f5f0e8; padding: 16px 32px; text-align: center; }" +
                   "</style>" +
                   "</head>" +
                   "<body>" +

                   "<div class='card'>" +

                   "<div class='header'>" +
                   "<h1>University Asset Management</h1>" +
                   "<p>Maintenance Assignment Notification</p>" +
                   "</div>" +

                   "<div class='body'>" +

                   "<p>" +
                   "Dear <strong>" + model.TechnicianName + "</strong>,<br/>" +
                   "You have been assigned a new maintenance job." +
                   "</p>" +

                   "<p><strong>Ticket ID:</strong> " + model.TicketId + "</p>" +
                   "<p><strong>Asset:</strong> " + model.AssetName + "</p>" +
                   "<p><strong>College:</strong> " + model.CollegeName + "</p>" +
                   "<p><strong>Location:</strong> " + model.LocationName + "</p>" +
                   "<p><strong>Assigned By:</strong> " + model.AssignedBy + "</p>" +
                   "<p><strong>Assigned On:</strong> " +
                   model.AssignedAt.ToString("dd MMM yyyy, h:mm tt") +
                   "</p>" +

                   "<p><strong>Issue Description:</strong></p>" +

                   "<div>" +
                   model.IssueDescription +
                   "</div>" +

                   "<p>Status: <strong>Assigned</strong></p>" +

                   "</div>" +

                   "<div class='footer'>" +
                   "&copy; " + DateTime.Now.Year +
                   " University Asset Management System" +
                   "</div>" +

                   "</div>" +
                   "</body>" +
                   "</html>";
        }



        ////        private static string BuildEmailBody(AssignmentEmailModel model) => $"""
        ////<!DOCTYPE html>
        ////<html>
        ////<head>
        ////    <meta charset="utf-8"/>
        ////    <style>
        ////        body {{font - family: Arial, sans-serif;
        ////            background: #f5f0e8;
        ////            margin: 0;
        ////            padding: 24px;
        ////        }}

        ////        .card {{background: #fff;
        ////            border-radius: 12px;
        ////            max-width: 560px;
        ////            margin: 0 auto;
        ////            overflow: hidden;
        ////            box-shadow: 0 2px 12px rgba(0,0,0,.08);
        ////        }}

        ////        .header {{background: #0e1525;
        ////            padding: 28px 32px;
        ////        }}

        ////        .header h1 {{color: #c9a84c;
        ////            font-size: 20px;
        ////            margin: 0;
        ////        }}

        ////        .header p {{color: #94a3b8;
        ////            font-size: 13px;
        ////            margin: 6px 0 0;
        ////        }}

        ////        .body {{padding: 28px 32px;
        ////        }}

        ////        .greeting {{font-size: 15px;
        ////            color: #0e1525;
        ////            margin-bottom: 18px;
        ////        }}

        ////        .row {{display: flex;
        ////            justify-content: space-between;
        ////            padding: 11px 0;
        ////            border-bottom: 1px solid #ede8de;
        ////        }}

        ////        .row:last-of-type {{border-bottom: none;
        ////        }}

        ////        .row .lbl {{font-size: 12.5px;
        ////            color: #7a8a9e;
        ////        }}

        ////        .row .val {{font-size: 13.5px;
        ////            font-weight: 600;
        ////            color: #0e1525;
        ////        }}

        ////        .issue-box {{background: #f5f0e8;
        ////            border-radius: 8px;
        ////            padding: 14px 16px;
        ////            margin: 18px 0;
        ////            font-size: 13.5px;
        ////            color: #0e1525;
        ////            line-height: 1.6;
        ////        }}

        ////        .badge {{display: inline-block;
        ////            padding: 4px 12px;
        ////            border-radius: 20px;
        ////            font-size: 12px;
        ////            font-weight: 600;
        ////            background: rgba(47,184,160,.1);
        ////            color: #1a7a6a;
        ////        }}

        ////        .footer {{background: #f5f0e8;
        ////            padding: 16px 32px;
        ////            font-size: 12px;
        ////            color: #7a8a9e;
        ////            text-align: center;
        ////        }}
        ////    </style>
        ////</head>

        ////<body>
        ////    <div class="card">

        ////        <div class="header">
        ////            <h1>University Asset Management</h1>
        ////            <p>Maintenance Assignment Notification</p>
        ////        </div>

        ////        <div class="body">

        ////            <p class="greeting">
        ////                Dear <strong>{model.TechnicianName}</strong>,<br/>
        ////                You have been assigned a new maintenance job.
        ////                Please review the details below and proceed accordingly.
        ////            </p>

        ////            <div class="row">
        ////                <span class="lbl">Ticket ID</span>
        ////                <span class="val">{model.TicketId}</span>
        ////            </div>

        ////            <div class="row">
        ////                <span class="lbl">Asset</span>
        ////                <span class="val">{model.AssetName}</span>
        ////            </div>

        ////            <div class="row">
        ////                <span class="lbl">College</span>
        ////                <span class="val">{model.CollegeName}</span>
        ////            </div>

        ////            <div class="row">
        ////                <span class="lbl">Location</span>
        ////                <span class="val">{model.LocationName}</span>
        ////            </div>

        ////            <div class="row">
        ////                <span class="lbl">Assigned By</span>
        ////                <span class="val">{model.AssignedBy}</span>
        ////            </div>

        ////            <div class="row">
        ////                <span class="lbl">Assigned On</span>
        ////                <span class="val">{model.AssignedAt:dd MMM yyyy, h:mm tt}</span>
        ////            </div>

        ////            <p style="font-size:13px;color:#7a8a9e;margin:18px 0 6px">
        ////                Issue Description:
        ////            </p>

        ////            <div class="issue-box">
        ////                {model.IssueDescription}
        ////            </div>

        ////            <p style="font-size:13px;color:#7a8a9e;margin-top:18px">
        ////                Status: <span class="badge">Assigned</span>
        ////            </p>

        ////            <p style="font-size:13px;color:#7a8a9e;margin-top:16px;line-height:1.6">
        ////                Please contact the college administration if you need
        ////                further information about this assignment.
        ////            </p>

        ////        </div>

        ////        <div class="footer">
        ////            &copy; {DateTime.Now.Year} University Asset Management System
        ////            &mdash; This is an automated notification.
        ////        </div>

        ////    </div>
        ////</body>
        ////</html>
        ////""";
    }
}
