﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Net;
using System.Net.Mail;
using System.Text.Json;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions;

public class SendMyEmailAction : BaseAction
{
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var smtp_settings_name = More.GetValue("smtp_settings_name").ThrowIfBlank("smtp_settings_name");

        var smtp_settings = await DbHelper.FromProcAsync<SmtpSettings>("WJbSettings_Get", smtp_settings_name, cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(smtp_settings);

        var from = More.GetValue("from");
        if (string.IsNullOrEmpty(from)) from = smtp_settings.From;
        ArgumentNullException.ThrowIfNull(from);

        var to = More.GetValue("to");
        ArgumentNullException.ThrowIfNull(to);

        var cc = More.GetValue("cc");
        var bcc = More.GetValue("bcc");

        var subject = More.GetValue("subject");
        var body = More.GetValue("body");

        var attachment = More.GetValue("attachment");
        var attachments = More.GetValue("attachments");

        await LogHelper.LogDebugAsync(nameof(SendEmailAction), new { jobId = JobId, to, cc, bcc, subject, body = ShortStr(body, 200), attachment, attachments }, cancellationToken);

        if (Guid.TryParse(body, out var guidBody)) body = await WJbFileHelper.GetAsync(body, cancellationToken);

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
                var file = await WJbFileHelper.GetAsync(guidAttach, cancellationToken);

                if (file?.FileContent != null) message.Attachments.Add(new Attachment(new MemoryStream(file.FileContent), file.FileName));
            }
            else
            {
                message.Attachments.Add(new Attachment(attachment));
            }
        }
        else if (!string.IsNullOrEmpty(attachments))
        {
            foreach (var fileName in JsonSerializer.Deserialize<object[]>(attachments) ?? Enumerable.Empty<object>())
            {
                attachment = Convert.ToString(fileName);

                if (Guid.TryParse(attachment, out var guidAttach))
                {
                    var file = await WJbFileHelper.GetAsync(guidAttach, cancellationToken);

                    if (file?.FileContent != null) message.Attachments.Add(new Attachment(new MemoryStream(file.FileContent), file.FileName));
                }
                else if (!string.IsNullOrWhiteSpace(attachment))
                {
                    message.Attachments.Add(new Attachment(attachment));
                }
            }
        }

        using var smtp = new SmtpClient(smtp_settings.Host, smtp_settings.Port);

        smtp.EnableSsl = smtp_settings.EnableSsl;
        smtp.Credentials = new NetworkCredential(smtp_settings.UserName, smtp_settings.Password);

        await smtp.SendMailAsync(message, cancellationToken);

        await LogHelper.LogInformationAsync(nameof(SendEmailAction), new { jobId = JobId, result = "OK" }, cancellationToken);

        return true;
    }
}
