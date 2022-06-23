// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions.SshNet;

public class GetFilesAction : SshNetAction
{
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
    {
        const string funcName = "SshNet_GetFiles";

        var jobId = JobId;

        var sshnet_options_name = More.GetValue("sshnet_options_name").ThrowIfBlank("sshnet_options_name");

        var remote_path = More.GetValue("remote_path");
        if (string.IsNullOrEmpty(remote_path)) remote_path = ".";

        var local_path = More.GetValue("local_path", "");

        var local_base_path = More.GetValue("local_base_path");
        if (!string.IsNullOrEmpty(local_base_path))
        {
            local_path = Path.Combine(local_base_path, local_path);
        }

        using var sftp = await CreateSftpClient(sshnet_options_name);
        {
            sftp.Connect();

            var files = await sftp.ListDirectoryAsync(remote_path);

            foreach (var file in files.Where(e => e.IsRegularFile).OrderBy(o => o.LastWriteTime))
            {
                if (cancellationToken.IsCancellationRequested) break;

                try
                {
                    string remoteFile = $"{remote_path}/{file.Name}", localFile = Path.Combine(local_path, file.Name);

                    await sftp.DownloadFileAsync(remoteFile, localFile, cancellationToken);

                    await sftp.DeleteFileAsync(remoteFile, cancellationToken);

                    await LogHelper.LogInformationAsync(funcName, new { jobId, errMsg = $"Downloaded: {file.Name}" }, cancellationToken);
                }
                catch (Exception ex)
                {
                    await LogHelper.LogErrorAsync(funcName, new { jobId, errMsg = $"Failed: {file.Name}. Error: {ex.Message}" }, cancellationToken);
                    throw;
                }
            }

            sftp.Disconnect();
        }

        await LogHelper.LogInformationAsync(funcName, new { jobId = JobId, result = "OK" }, cancellationToken);

        return true;
    }
}