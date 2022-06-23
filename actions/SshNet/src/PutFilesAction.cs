// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions.SshNet;

public class PutFilesAction : SshNetAction
{
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
    {
        const string funcName = "SshNet_PutFiles";

        var jobId = JobId;

        var sshnet_options_name = More.GetValue("sshnet_options_name").ThrowIfBlank("sshnet_options_name");

        var remote_path = More.GetValue("remote_path");
        if (string.IsNullOrEmpty(remote_path)) remote_path = ".";

        var local_path = More.GetValue("local_path", "");

        var local_move_path = More.GetValue("local_move_path");
        if (local_move_path.Equals("-delete", StringComparison.CurrentCultureIgnoreCase))
            local_move_path = string.Empty;
        
        bool remove = string.IsNullOrEmpty(local_move_path);

        var local_base_path = More.GetValue("local_base_path");
        if (!string.IsNullOrEmpty(local_base_path))
        {
            local_path = Path.Combine(local_base_path, local_path);
            if (!string.IsNullOrEmpty(local_move_path)) local_move_path = Path.Combine(local_base_path, local_move_path);
        }

        using var sftp = await CreateSftpClient(sshnet_options_name);
        {
            sftp.Connect();

            var files = new DirectoryInfo(local_path).GetFiles();

            foreach (var file in files.OrderBy(o => o.LastWriteTime))
            {
                if (cancellationToken.IsCancellationRequested) break;

                try
                {
                    string remoteFile = $"{remote_path}/{file.Name}", localFile = Path.Combine(local_path, file.Name);

                    await sftp.UploadFileAsync(localFile, remoteFile);

                    await LogHelper.LogInformationAsync(funcName, new { jobId, errMsg = $"Uploaded: {file.Name}" });

                    if (!string.IsNullOrEmpty(local_move_path))
                    {
                        System.IO.File.Move(file.FullName, Path.Combine(local_move_path, file.Name), true);
                        await LogHelper.LogDebugAsync(funcName, new { jobId, errMsg = $"Moved: {file.Name}" });
                    }
                    else
                    {
                        System.IO.File.Delete(file.FullName);
                        await LogHelper.LogDebugAsync(funcName, new { jobId, errMsg = $"Deleted: {file.Name}" });
                    }
                }
                catch (Exception ex)
                {
                    await LogHelper.LogErrorAsync(funcName, new { jobId, errMsg = $"Failed: {file.Name}. Error: {ex.Message}" });
                    throw;
                }
            }

            sftp.Disconnect();
        }

        await LogHelper.LogInformationAsync(funcName, new { jobId = JobId, result = "OK" }, cancellationToken);

        return true;
    }
}