// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Renci.SshNet;
using Renci.SshNet.Common;
using UkrGuru.SqlJson;

namespace UkrGuru.WebJobs.Actions.SshNet;

public class SshNetAction : BaseAction
{
    public async Task<SftpClient> CreateSftpClient(string sshnet_options_name, CancellationToken cancellationToken = default)
    {
        SftpClient sftp = null;

        var sshnet_options = await DbHelper.FromProcAsync<SshNetOptions>("WJbSettings_Get", sshnet_options_name.ThrowIfBlank("sshnet_options_name"), cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(sshnet_options);

        if (sshnet_options.KeyFiles?.Length > 0)
        {
            sftp = new SftpClient(sshnet_options.Host, sshnet_options.Port, sshnet_options.UserName, sshnet_options.KeyFiles);
        }
        else
        {
            sftp = new SftpClient(sshnet_options.Host, sshnet_options.Port, sshnet_options.UserName, sshnet_options.Password);
        }

        if (!string.IsNullOrEmpty(sshnet_options.FingerPrint))
        {
            // string hexFingerPrint;
            sftp.HostKeyReceived += delegate (object sender, HostKeyEventArgs e)
            {
                var bytes = sshnet_options.FingerPrint.Split(':').Select(s => Convert.ToByte(s, 16)).ToArray();
                if (e.FingerPrint.SequenceEqual(bytes))
                    e.CanTrust = true;
                else
                    e.CanTrust = false;

                //var fingerPrint = new StringBuilder();
                //foreach (var b in e.FingerPrint) fingerPrint.AppendFormat("{0:x2}", b);
                //hexFingerPrint = String.Join(':', fingerPrint);
            };
        }

        return await Task.FromResult(sftp);
    }
}