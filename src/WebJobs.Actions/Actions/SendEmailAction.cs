﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions
{
    public class SmtpSettings
    {
        [JsonPropertyName("from")]
        public string From { get; set; }
        [JsonPropertyName("host")]
        public string Host { get; set; }
        [JsonPropertyName("port")]
        public int Port { get; set; }
        [JsonPropertyName("enableSsl")]
        public bool EnableSsl { get; set; }
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class SendEmailAction : BaseAction
    {
        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            var smtp_settings_name = More.GetValue("smtp_settings_name");
            if (string.IsNullOrWhiteSpace(smtp_settings_name)) throw new ArgumentNullException(nameof(smtp_settings_name));

            var smtp_settings = await DbHelper.FromProcAsync<SmtpSettings>("WJbSettings_Get", new { Name = smtp_settings_name }, cancellationToken: cancellationToken);
            if (string.IsNullOrWhiteSpace(smtp_settings?.Host)) throw new ArgumentNullException("smtp_settings");

            var from = More.GetValue("from");
            if (string.IsNullOrWhiteSpace(from)) from = smtp_settings.From;
            if (string.IsNullOrWhiteSpace(from)) throw new ArgumentNullException(nameof(from));

            var to = More.GetValue("to");
            if (string.IsNullOrWhiteSpace(to)) throw new ArgumentNullException(nameof(to));

            var cc = More.GetValue("cc");
            var bcc = More.GetValue("bcc");

            var subject = More.GetValue("subject");
            var body = More.GetValue("body");

            var attachment = More.GetValue("attachment");
            var attachments = More.GetValue("attachments");

            await LogHelper.LogDebugAsync("SendEmailAction", new { jobId = JobId, to, cc, bcc, subject, body = ShortStr(body, 200), attachment, attachments });

            if (Guid.TryParse(body, out var guidBody))
            {
                var file = await DbHelper.FromProcAsync<File>("WJbFiles_Item", body);
                if (file.Id != Guid.Empty) body = Encoding.UTF8.GetString(file.FileContent);
            }

            MailMessage message = new(from, to, subject, body)
            {
                IsBodyHtml = Utility.IsHtmlBody(body)
            };

            if (!string.IsNullOrEmpty(cc)) message.CC.Add(cc);

            if (!string.IsNullOrEmpty(bcc)) message.Bcc.Add(bcc);

            if (!string.IsNullOrEmpty(attachment))
            {
                if (Guid.TryParse(attachment, out var guidAttach))
                {
                    var file = await DbHelper.FromProcAsync<File>("WJbFiles_Item", attachment);
                    if (file.Id != Guid.Empty)
                    {
                        System.IO.MemoryStream ms = new (file.FileContent);
                        message.Attachments.Add(new Attachment(ms, file.FileName));
                    }
                }
                else
                {
                    message.Attachments.Add(new Attachment(attachment));
                }
            }
            else if (!string.IsNullOrEmpty(attachments))
            {
                var files = JsonSerializer.Deserialize<object[]>(attachments);
                for (var i = 0; i < files.Length; i++)
                {
                    var fileName = Convert.ToString(files[i]);
                    if (Guid.TryParse(fileName, out var guidAttach))
                    {
                        var file = await DbHelper.FromProcAsync<File>("WJbFiles_Item", fileName);
                        if (file.Id != Guid.Empty)
                        {
                            System.IO.MemoryStream ms = new (file.FileContent);
                            message.Attachments.Add(new Attachment(ms, file.FileName));
                        }
                    }
                    else
                    {
                        message.Attachments.Add(new Attachment(fileName));
                    }
                }
            }

            using var smtp = new SmtpClient(smtp_settings.Host, smtp_settings.Port);

            smtp.EnableSsl = smtp_settings.EnableSsl;
            smtp.Credentials = new NetworkCredential(smtp_settings.UserName, smtp_settings.Password);

            await smtp.SendMailAsync(message);

            await LogHelper.LogDebugAsync("SendEmailAction", new { jobId = JobId, result = "OK" });

            return true;
        }
    }
}