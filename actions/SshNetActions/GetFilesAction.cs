using Renci.SshNet;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions.SshNet
{
    public class SshNetGetFilesAction : BaseAction
    {
        public class SshNetOptions
        {
            public string Host { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            const string funcName = "SshNetGetFiles";

            var jobId = JobId;
            try
            {
                var sshnet_options_name = More.GetValue("sshnet_options_name");
                if (string.IsNullOrEmpty(sshnet_options_name)) throw new ArgumentNullException("sshnet_options_name");

                var sshnet_options = await DbHelper.FromProcAsync<SshNetOptions>("WJbSettings_Get", new { Name = sshnet_options_name }, cancellationToken: cancellationToken);
                if (string.IsNullOrEmpty(sshnet_options?.Host)) throw new ArgumentNullException("sshnet_options");

                var remote_path = More.GetValue("remote_path");
                if (string.IsNullOrEmpty(remote_path)) throw new ArgumentNullException("remote_path");

                var local_path = More.GetValue("local_path");
                if (string.IsNullOrEmpty(local_path)) throw new ArgumentNullException("local_path");

                var local_base_path = More.GetValue("local_base_path");
                if (string.IsNullOrEmpty(local_base_path)) throw new ArgumentNullException("local_base_path");

                if (!string.IsNullOrEmpty(local_base_path))
                    local_path = Path.Combine(local_base_path, local_path);

                var remove = More.GetValue("remove", false) ?? false;

                var connectionInfo = new ConnectionInfo(sshnet_options.Host, sshnet_options.UserName, 
                    new PasswordAuthenticationMethod(sshnet_options.UserName, sshnet_options.Password));

                using var client = new SftpClient(connectionInfo);
                try
                {
                    // client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(100);  maybe in future
                    client.Connect();
                    
                    // if (client.IsConnected) maybe in future

                    var files = client.ListDirectory(remote_path);
                    foreach (var sftpFile in files)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        if (sftpFile.IsRegularFile)
                        {
                            string remoteFile = $"{remote_path}/{sftpFile.Name}", localFile = Path.Combine(local_path, sftpFile.Name);

                            try
                            {
                                using var stream = System.IO.File.OpenWrite(localFile);
                                {
                                    client.DownloadFile(remoteFile, stream);

                                    if (remove) client.DeleteFile(remoteFile);
                                }

                                await LogHelper.LogInformationAsync(funcName, new { jobId, errMsg = $"Downloaded: {sftpFile.Name}" });
                            }
                            catch (Exception ex)
                            {
                                await LogHelper.LogErrorAsync(funcName, new { jobId, errMsg = $"Fail: {sftpFile.Name}. Error: {ex.Message}" });
                                throw;
                            }
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