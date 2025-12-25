using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.Text;
using TinyCity.BookmarkEngines;
using TinyCity.Commands;
using TinyCity.Services;

namespace TinyCity
{
    public class Program
    {
        async static Task<int> Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // emoji support

            var stopWatch = Stopwatch.StartNew();
            var extraArgHandler = new ExtraArgumentHandler();

            var services = SetupIoC();
            var serviceProvider = services.BuildServiceProvider();

            var rootCommand = new RootCommand("A command line tool for searching, importing, and exporting bookmarks");
            
            var searchCommandInstance = serviceProvider.GetRequiredService<SearchCommandHandler>();
            var listCommandInstance = serviceProvider.GetRequiredService<ListCommandHandler>();
            var configCommandInstance = serviceProvider.GetRequiredService<ConfigCommandHandler>();
            var exportCommandInstance = serviceProvider.GetRequiredService<ExportCommandHandler>();
            var importCommandInstance = serviceProvider.GetRequiredService<ImportCommandHandler>();

            var searchCommand = searchCommandInstance.CreateCommand(extraArgHandler);
            var listCommand = listCommandInstance.CreateCommand(extraArgHandler);
            var configCommand = configCommandInstance.CreateCommand(extraArgHandler);
            var exportCommand = exportCommandInstance.CreateCommand(extraArgHandler);
            var importCommand = importCommandInstance.CreateCommand(extraArgHandler);

            rootCommand.AddCommand(searchCommand);
            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(configCommand);
            rootCommand.AddCommand(exportCommand);
            rootCommand.AddCommand(importCommand);

            var commandLineBuilder = new CommandLineBuilder(rootCommand)
                .UseVersionOption()
                .UseHelp()
                .UseParseErrorReporting()
                .UseExceptionHandler()
                .CancelOnProcessTermination();

            var parser = commandLineBuilder.Build();
            int result = await parser.InvokeAsync(args);
            
            stopWatch.Stop();
            extraArgHandler.ShowOutput(stopWatch);
            
            return result;
        }

        static ServiceCollection SetupIoC()
        {
            var settings = TinyCitySettings.Load();

            var services = new ServiceCollection();
            services.AddSingleton<ChromeBookmarks>();
            services.AddSingleton<MarkdownBookmarks>();
            services.AddSingleton<HtmlBookmarks>();
            services.AddSingleton<BookmarkAggregator>();
            services.AddSingleton<TinyCitySettings>(settings);
            services.AddSingleton<ConfigCommandHandler>();
            services.AddSingleton<SearchCommandHandler>();
            services.AddSingleton<ListCommandHandler>();
            services.AddSingleton<FileBackupService>();
            services.AddSingleton<BookmarkImporter>();
            services.AddSingleton<ExportCommandHandler>();
            services.AddSingleton<ImportCommandHandler>();

            return services;
        }
    }
}