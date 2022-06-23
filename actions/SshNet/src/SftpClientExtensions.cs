// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace UkrGuru.WebJobs.Actions.SshNet;

public static class SftpClientExtensions
{
    public static Task<IEnumerable<SftpFile>> ListDirectoryAsync(this SftpClient sftp, string remote_path, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            return sftp.ListDirectory(remote_path);
        }, cancellationToken);
    }

    public static Task DeleteFileAsync(this SftpClient sftp, string remote_path, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            sftp.DeleteFile(remote_path);
        }, cancellationToken);
    }

    public static Task DownloadFileAsync(this SftpClient sftp, string remote_path, string local_path, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            using (var stream = File.OpenWrite(local_path))
            {
                sftp.DownloadFile(remote_path, stream);
            }
        }, cancellationToken);
    }

    public static Task UploadFileAsync(this SftpClient sftp, string local_path, string remote_path, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            using (var stream = File.OpenRead(local_path))
            {
                sftp.UploadFile(stream, remote_path);
            }
        }, cancellationToken);
    }
}