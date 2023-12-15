using Microsoft.Extensions.Hosting;

namespace Test
{
    internal class App : IHostedService
    {
        readonly AWSFileSystem.AWSFileSystem _fileSystem;

        public App(AWSFileSystem.AWSFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _fileSystem.MakeSureBucketExists();

            var cancelationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            //var fileInfo = new FileInfo("c:\\tmp\\3190930_Mortgage1.pdf");
            //long fileSize = fileInfo.Length;
            //using FileStream st = fileInfo.OpenRead();
            //await _fileSystem.UploadStream("3190930_Mortgage1.pdf", fileSize, st);

            using FileStream st2 = new FileStream("c:\\tmp\\3190930_Mortgage_2.pdf", FileMode.CreateNew, FileAccess.Write);
            await _fileSystem.GetObject("3190930_Mortgage1.pdf", st2);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
