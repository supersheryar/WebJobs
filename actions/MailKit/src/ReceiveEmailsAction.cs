﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using MailKit.Net.Pop3;
using MimeKit;
using System.Text;
using System.Text.Json;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions.MailKit;

public class ReceiveEmailsAction : BaseAction
{
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        const string funcName = "MailKit.ReceiveEmails";

        var jobId = JobId;

        var pop3_settings_name = More.GetValue("pop3_settings_name").ThrowIfBlank("pop3_settings_name");

        var pop3_settings = await DbHelper.FromProcAsync<Pop3Settings>("WJbSettings_Get", pop3_settings_name, cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(pop3_settings?.Host);

        var proc_rule = More.GetValue("proc_rule");

        using (var client = new Pop3Client())
        {
            await client.ConnectAsync(pop3_settings.Host, pop3_settings.Port, pop3_settings.UseSsl, cancellationToken: cancellationToken);
            await client.AuthenticateAsync(pop3_settings.UserName, pop3_settings.Password, cancellationToken: cancellationToken);

            for (int i = 0; i < client.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested) break;

                var message = await client.GetMessageAsync(i, cancellationToken: cancellationToken);

                var fileBody = new Data.File()
                {
                    FileName = $"{pop3_settings_name}-" + message.HtmlBody == null ? "body.txt" : "body.html",
                    FileContent = Encoding.UTF8.GetBytes(message.TextBody ?? message.HtmlBody)
                };

                var body = await fileBody.SetAsync(cancellationToken);

                var attachments = new List<string>();
                foreach (var attachment in message.Attachments)
                {
                    if (attachment is MimePart part)
                    {
                        var ms = new MemoryStream();
                        await part.Content.DecodeToAsync(ms, cancellationToken: cancellationToken);

                        var file = new Data.File() { FileName = $"{pop3_settings_name}-{part.FileName}", FileContent = ms.ToArray() };

                        var fileId = await file.SetAsync(cancellationToken);

                        attachments.Add(fileId);

                        await LogHelper.LogInformationAsync(funcName, new { jobId, result = $"Added Email Attachment: {fileId}." });
                    }
                }

                if (!string.IsNullOrEmpty(proc_rule))
                {
                    var email_jobId = await DbHelper.FromProcAsync<int?>("WJbQueue_Ins", new
                    {
                        Rule = proc_rule,
                        RulePriority = (byte)Priorities.ASAP,
                        RuleMore = JsonSerializer.Serialize(new
                        {
                            from = message.From.ToString(),
                            to = message.To.ToString(),
                            subject = message.Subject,
                            body,
                            attachments = attachments.ToArray()
                        })
                    }, cancellationToken: cancellationToken);

                    await LogHelper.LogInformationAsync(funcName, new { jobId, result = $"Added Email Job: {email_jobId}." });
                }

                await client.DeleteMessageAsync(i, cancellationToken: cancellationToken);
            }

            await client.DisconnectAsync(true, cancellationToken: cancellationToken);
        }

        await LogHelper.LogInformationAsync(funcName, new { jobId = JobId, result = "OK" }, cancellationToken);

        return true;
    }
}