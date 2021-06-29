using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions.SshNet
{
    public class SshNetPutFilesAction : BaseAction
    {
        public class SshNetOptions
        {
            public string Host { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            const string funcName = "SshNetPutFiles";

            var jobId = JobId;
            try
            {
                var sshnet_options_name = More.GetValue("sshnet_options_name");
                if (string.IsNullOrEmpty(sshnet_options_name)) throw new ArgumentNullException("sshnet_options_name");

                var sshnet_options = await DbHelper.FromProcAsync<SshNetOptions>("WJbSettings_Get", new { Name = sshnet_options_name }, cancellationToken: cancellationToken);
                if (string.IsNullOrEmpty(sshnet_options?.Host)) throw new ArgumentNullException("sshnet_options");

                var local_path = More.GetValue("local_path");
                if (string.IsNullOrEmpty(local_path)) throw new ArgumentNullException("local_path");

                var local_move_path = More.GetValue("local_move_path");
                if (local_move_path.Equals("-delete", StringComparison.CurrentCultureIgnoreCase))
                    local_move_path = string.Empty;

                var remote_path = More.GetValue("remote_path");
                if (string.IsNullOrEmpty(remote_path)) throw new ArgumentNullException("remote_path");

                bool remove = string.IsNullOrEmpty(local_move_path);

                var local_base_path = More.GetValue("local_base_path");
                if (string.IsNullOrEmpty(local_base_path)) throw new ArgumentNullException("local_base_path");

                if (!string.IsNullOrEmpty(local_base_path))
                {
                    local_path = Path.Combine(local_base_path, local_path);
                    if (!string.IsNullOrEmpty(local_move_path)) local_move_path = Path.Combine(local_base_path, local_move_path);
                }

                var connectionInfo = new ConnectionInfo(sshnet_options.Host, sshnet_options.UserName, 
                    new PasswordAuthenticationMethod(sshnet_options.UserName, sshnet_options.Password));

                using var client = new SftpClient(connectionInfo);
                try
                {
                    // client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(100);  maybe in future
                    client.Connect();

                    // if (client.IsConnected) maybe in future
                    
                    IEnumerable<FileSystemInfo> infos =
                        new DirectoryInfo(local_path).EnumerateFileSystemInfos();

                    foreach (FileSystemInfo info in infos)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        if (info.Attributes.HasFlag(FileAttributes.Directory)) continue;

                        try
                        {
                            using (Stream fileStream = new FileStream(info.FullName, FileMode.Open))
                                client.UploadFile(fileStream, $"{remote_path}/{info.Name}");

                            await LogHelper.LogInformationAsync(funcName, new { jobId, errMsg = $"Uploaded: {info.Name}" });

                            if (!string.IsNullOrEmpty(local_move_path))
                            {
                                File.Move(info.FullName, Path.Combine(local_move_path, info.Name), true);
                                await LogHelper.LogDebugAsync(funcName, new { jobId, errMsg = $"Moved: {info.Name}" });
                            }
                            else
                            {
                                File.Delete(info.FullName);
                                await LogHelper.LogDebugAsync(funcName, new { jobId, errMsg = $"Deleted: {info.Name}" });
                            }
                        }
                        catch (Exception ex)
                        {
                            await LogHelper.LogErrorAsync(funcName, new { jobId, errMsg = $"Fail: {info.Name}. Error: {ex.Message}" });
                            throw;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    client.Disconnect();
                }

                return true;
            }
            catch (Exception ex)
            {
                await LogHelper.LogErrorAsync(funcName, new { jobId, errMsg = ex.Message });
                return false;
            }
        }
    }
}