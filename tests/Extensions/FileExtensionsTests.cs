using Xunit;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using UkrGuru.WebJobs.Data;

namespace WebJobsTests.Extensions
{
    public class FileExtensionsTests
    {
        [Theory]
        [InlineData("<html><head></head><title>TEST</title><body></body></html>")]
        public async Task CompressDecompressTest(string content, CancellationToken cancellationToken = default)
        {
            var sourceBytes = Encoding.UTF8.GetBytes(content);

            File file = new() { FileName = "file.html", FileContent = sourceBytes }; 

            await file.CompressAsync(cancellationToken);

            await file.DecompressAsync(cancellationToken);

            Assert.Equal(sourceBytes, file.FileContent);
        }
    }
}