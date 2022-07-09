using Xunit;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using UkrGuru.WebJobs.Data;

namespace WebJobsTests.Extensions;

public class FileExtensionsTests
{
    [Theory]
    [InlineData("file.html", "<html><head></head><title>TEST</title><body></body></html>")]
    public async Task CompressDecompressTest(string filename, string content, CancellationToken cancellationToken = default)
    {
        var sourceBytes = Encoding.UTF8.GetBytes(content);

        File file = new() { FileName = filename, FileContent = sourceBytes }; 

        await file.CompressAsync(cancellationToken);

        Assert.EndsWith(".gzip", file.FileName);

        await file.DecompressAsync(cancellationToken);

        Assert.Equal(filename, file.FileName);

        Assert.Equal(sourceBytes, file.FileContent);
    }
}