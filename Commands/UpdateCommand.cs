using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.CommandLine;
using TinyCity.Commands.Settings;

namespace TinyCity.Commands
{

    public class UpdateCommand : BaseCommand<UpdateCommandSettings>
    {
        public UpdateCommand()
        {
        }
        
        public override Task<int> ExecuteAsync(UpdateCommandSettings settings)
        {
            string fileUrl = "";

            switch (Environment.OSVersion.Platform)
            {   
                case PlatformID.Win32NT:
                    fileUrl = "https://github.com/yetanotherchris/tiny-city/releases/latest/download/tinycity.exe";
                    break;
                case PlatformID.Unix:
                    fileUrl = "https://github.com/yetanotherchris/tiny-city/releases/latest/download/tinycity";
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(fileUrl))
            {
                AnsiConsole.MarkupLine("[red]Unsupported OS.[/]");
                return Task.FromResult(1);
            }

            var processPath = Environment.ProcessPath;
            if (string.IsNullOrEmpty(processPath))
            {
                AnsiConsole.MarkupLine("[red]Could not determine current process path.[/]");
                return Task.FromResult(1);
            }

            // Download the file
            string downloadFilename = $"{processPath}.new";
            var task = Task.Run(async () =>
            {
                AnsiConsole.MarkupLine($"[green]Url: '{fileUrl}'.[/]");
                await Download(downloadFilename, fileUrl);
            });
            task.Wait(TimeSpan.FromMinutes(5));

            // Backup the current process
            string backupFilename = $"{processPath}.bak";
            if (Path.Exists(backupFilename))
            {
                File.Delete(backupFilename);
            }
            var processFileInfo = new FileInfo(processPath);
            processFileInfo.MoveTo(backupFilename);

            // Rename the downloaded file to the current process filename
            string newFilename = downloadFilename.Replace(".new", "");
            var downloadedFileInfo = new FileInfo(downloadFilename);
            downloadedFileInfo.MoveTo(newFilename);

            // ...tinycity.bak removal is done on startup

            return Task.FromResult(0);
        }

        private async Task Download(string localFilename, string fileUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using (FileStream fileStream = new FileStream(localFilename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await CopyWithProgressAsync(response.Content, fileStream);
                }

                AnsiConsole.MarkupLine("File downloaded successfully!");
            }
        }

        static async Task CopyWithProgressAsync(HttpContent content, Stream destination)
        {
            long totalBytes = content.Headers.ContentLength ?? -1;
            long totalBytesCopied = 0;
            byte[] buffer = new byte[8092];
            int bytesRead;

            var progressBar = AnsiConsole.Progress()
                .Columns(new ProgressColumn[]
                        {
                            new TaskDescriptionColumn(),
                            new ProgressBarColumn(),    
                            new PercentageColumn(),     
                            new RemainingTimeColumn(),  
                            new SpinnerColumn(),        
                            new TransferSpeedColumn(),  
                        });

            await progressBar.StartAsync(async ctx =>
            {
                var task = ctx.AddTask($"[green]Downloading {totalBytes} bytes:[/]");
                task.MaxValue(totalBytes);
                task.StartTask();

                while (!ctx.IsFinished)
                {
                    using (Stream sourceStream = await content.ReadAsStreamAsync())
                    {
                        while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await destination.WriteAsync(buffer, 0, bytesRead);
                            totalBytesCopied += bytesRead;

                            if (bytesRead > 0)
                            {
                                task.Increment(bytesRead);
                            }
                        }
                    }
                }
            });
        }

        public static Command CreateCommand(IServiceProvider serviceProvider, ExtraInfoInterceptor interceptor, Action<Exception> onException)
        {
            var command = new Command("update", "Updates Tinycity, downloading the latest release from Github.");

            var settingsBinder = new UpdateCommandSettings();
            settingsBinder.ConfigureCommand(command);

            command.SetHandler(async (UpdateCommandSettings settings) =>
            {
                try
                {
                    var updateCommandInstance = serviceProvider.GetRequiredService<UpdateCommand>();
                    interceptor.SetShowExtraInfo(settings.Extra);
                    await updateCommandInstance.ExecuteAsync(settings);
                }
                catch (Exception ex)
                {
                    onException(ex);
                }
            }, settingsBinder);

            return command;
        }
    }
}
