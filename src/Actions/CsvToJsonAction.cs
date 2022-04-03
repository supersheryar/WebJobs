using CsvHelper;
using System.Globalization;
using System.Text;
using System.Text.Json;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace UkrGuru.WebJobs.Actions;

public class CsvToJsonAction : BaseAction
{
    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
    {
        const string funcName = nameof(CsvToJsonAction);

        var jobId = JobId;
        try
        {
            Data.File fileJson = new() { FileName = "file.json" };

            var dataSourceId = More.GetValue("dataSourceId", 0);

            var sourceFileId = More.GetValue("sourceFileId");

            var header = await DbHelper.FromProcAsync<string>("api.ClientDataSources_Get_Header", dataSourceId, cancellationToken: cancellationToken);

            var csv = string.Empty;

            if (Guid.TryParse(sourceFileId, out var guidCsv))
            {
                var fileCsv = await DbHelper.FromProcAsync<Data.File>("WJbFiles_Get", guidCsv.ToString(), cancellationToken: cancellationToken);
                ArgumentNullException.ThrowIfNull(fileCsv);
                ArgumentNullException.ThrowIfNull(fileCsv.FileContent);

                csv = Encoding.UTF8.GetString(fileCsv.FileContent);

                if (!string.IsNullOrEmpty(fileCsv.FileName)) fileJson.FileName = Path.ChangeExtension(fileCsv.FileName, ".json");
            }

            if (!string.IsNullOrEmpty(header) && !csv.StartsWith(header, StringComparison.CurrentCultureIgnoreCase))
                csv = header + "\r\n" + csv;

            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

            using TextReader txtReader = new StreamReader(stream);
            using var csvReader = new CsvReader(txtReader, CultureInfo.InvariantCulture);

            var records = csvReader.GetRecords<dynamic>();

            var json = JsonSerializer.Serialize(records);

            var fileId = await DbHelper.FromProcAsync<Guid?>("WJbFiles_Ins",
                new { fileJson.FileName, FileContent = Encoding.UTF8.GetBytes(json) }, cancellationToken: cancellationToken);

            await DbHelper.ExecProcAsync("api.ConnFiles_Ins_Json",
                new { DataSourceId = dataSourceId, SourceFileId = sourceFileId, JsonFileId = fileId }, cancellationToken: cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            await LogHelper.LogErrorAsync(funcName, new { jobId, errMsg = ex.Message }, cancellationToken);
            return false;
        }
    }
}