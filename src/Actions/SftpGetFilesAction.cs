using Renci.SshNet;
using Renci.SshNet.Common;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions;

public class SftpGetFilesAction : BaseAction
{
    public class SftpOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FingerPrint { get; set; }
    }

    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
    {
        const string funcName = nameof(SftpGetFilesAction);

        var jobId = JobId;
        try
        {
            var dataSourceId = More.GetValue("dataSourceId", 0);

            var sftp_options = await DbHelper.FromProcAsync<SftpOptions>("api.FtpSettings_Get", $"FtpSource{dataSourceId}", cancellationToken: cancellationToken);
            ArgumentNullException.ThrowIfNull(sftp_options);

            var remote_path = More.GetValue("remote_path");
            ArgumentNullException.ThrowIfNull(remote_path);

            var remove = More.GetValue("remove", false) ?? false;

            var timeout = More.GetValue("timeout", 60) ?? 60;

            using (SftpClient client = new(sftp_options.Host, sftp_options.Port, sftp_options.UserName, sftp_options.Password))
            {
                try
                {
                    if (!string.IsNullOrEmpty(sftp_options?.FingerPrint))
                    {
                        // string hexFingerPrint;
                        client.HostKeyReceived += delegate (object sender, HostKeyEventArgs e)
                        {
                            var bytes = sftp_options.FingerPrint.Split(':').Select(s => Convert.ToByte(s, 16)).ToArray();
                            if (e.FingerPrint.SequenceEqual(bytes))
                                e.CanTrust = true;
                            else
                                e.CanTrust = false;

                            //var fingerPrint = new StringBuilder();
                            //foreach (var b in e.FingerPrint) fingerPrint.AppendFormat("{0:x2}", b);
                            //hexFingerPrint = String.Join(':', fingerPrint);
                        };
                    }

                    client.Connect();

                    if (!client.IsConnected) throw new Exception("Can't connect.");

                    var files = client.ListDirectory(remote_path);

                    var expired = DateTime.Now.AddSeconds(timeout);

                    foreach (var file in files.Where(e => e.IsRegularFile).OrderBy(o => o.LastWriteTime))
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        try
                        {
                            string remoteFile = $"{remote_path}/{file.Name}"; //, localFile = Path.Combine(local_path, clientFile.Name);

                            // var content = client.ReadAllBytes(remoteFile);
                            MemoryStream stream = new(); client.DownloadFile(remoteFile, stream);

                            var fileId = await DbHelper.FromProcAsync<Guid?>("WJbFiles_Ins",
                                new { FileName = file.Name, FileContent = stream.ToArray() }, cancellationToken: cancellationToken);

                            // save ConnFile
                            await DbHelper.ExecProcAsync("api.ConnFiles_Ins_Source", 
                                new { ClientDataSourceId = dataSourceId, SourceFileId = fileId }, cancellationToken: cancellationToken);

                            if (remove) client.DeleteFile(remoteFile);

                            await LogHelper.LogInformationAsync(funcName, new { jobId, errMsg = $"Downloaded: {file.Name}" }, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            await LogHelper.LogErrorAsync(funcName, new { jobId, errMsg = $"Fail: {file.Name}. Error: {ex.Message}" }, cancellationToken);
                            throw;
                        }

                        if (expired < DateTime.Now) break;
                    }
                }
                catch (Exception ex)
                {
                    await LogHelper.LogErrorAsync(funcName, new { jobId, errMsg = $"Error: {ex.Message}" }, cancellationToken);
                    throw;
                }
                finally
                {
                    client.Disconnect();
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            await LogHelper.LogErrorAsync(funcName, new { jobId, errMsg = ex.Message }, cancellationToken);
            return false;
        }
    }
}