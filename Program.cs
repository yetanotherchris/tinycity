using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Invocation;
using TinyCity.BookmarkEngines;
using TinyCity.Commands;

namespace TinyCity
{
    public class Program
    {
        async static Task<int> Main(string[] args)
        {
            EnsureDownloadedBackupIsRemoved();
            Console.OutputEncoding = Encoding.UTF8; // emoji support

            var stopWatch = Stopwatch.StartNew();
            var interceptor = new ExtraInfoInterceptor();
            Exception? capturedException = null;

            var services = SetupIoC();
            var serviceProvider = services.BuildServiceProvider();

            var rootCommand = new RootCommand("A command line tool for searching bookmarks");
            
            // Create search command
            var searchCommand = new Command("search", "Search the bookmarks.");
            searchCommand.AddAlias("q");
            
            var queryArg = new Argument<string>("query", "The search term to look for in bookmarks. Enclose your search inside quotes, e.g. \"my search words\"");
            var extraOpt = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            var launchOpt = new Option<bool>(new[] { "-l", "--launch" }, "Launch the first bookmark found in your default browser. If no bookmarks are found, nothing will happen.");
            var searchUrlsOpt = new Option<bool>(new[] { "-u", "--urls" }, "Also search bookmark urls.");
            var exportOpt = new Option<bool>(new[] { "-e", "--export" }, "Exports the results as 'exported-bookmarks.md' to the same directory as tinycity.");
            var exportFormatOpt = new Option<string>("--export-format", () => "- [{name}]({url}) ({urlhost})", "When exporting, sets the format of each link");
            
            searchCommand.AddArgument(queryArg);
            searchCommand.AddOption(extraOpt);
            searchCommand.AddOption(launchOpt);
            searchCommand.AddOption(searchUrlsOpt);
            searchCommand.AddOption(exportOpt);
            searchCommand.AddOption(exportFormatOpt);
            
            var searchCommandInstance = new SearchCommand(serviceProvider);
            searchCommand.SetHandler(async (string query, bool extra, bool launch, bool searchUrls, bool export, string exportFormat) =>
            {
                try
                {
                    var settings = new SearchCommandSettings
                    {
                        Extra = extra,
                        Launch = launch,
                        SearchUrls = searchUrls,
                        Query = query ?? string.Empty,
                        Export = export,
                        ExportFormat = exportFormat ?? "- [{name}]({url}) ({urlhost})"
                    };
                    
                    interceptor.SetShowExtraInfo(settings.Extra);
                    await searchCommandInstance.ExecuteAsync(settings);
                }
                catch (Exception ex)
                {
                    capturedException = ex;
                }
            }, queryArg, extraOpt, launchOpt, searchUrlsOpt, exportOpt, exportFormatOpt);
            
            // Create list command
            var listCommand = new Command("ls", "List all bookmarks.");
            listCommand.AddAlias("list");
            
            var listExtraOpt = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            var listExportOpt = new Option<bool>(new[] { "-e", "--export" }, "Exports the results as 'exported-bookmarks.md' to the same directory as tinycity.");
            var listExportFormatOpt = new Option<string>("--export-format", () => "- [{name}]({url}) ({urlhost})", "When exporting, sets the format of each link");
            
            listCommand.AddOption(listExtraOpt);
            listCommand.AddOption(listExportOpt);
            listCommand.AddOption(listExportFormatOpt);
            
            var listCommandInstance = new ListCommand(serviceProvider);
            listCommand.SetHandler(async (bool extra, bool export, string exportFormat) =>
            {
                try
                {
                    var settings = new ListCommandSettings
                    {
                        Extra = extra,
                        Export = export,
                        ExportFormat = exportFormat ?? "- [{name}]({url}) ({urlhost})"
                    };
                    
                    interceptor.SetShowExtraInfo(settings.Extra);
                    await listCommandInstance.ExecuteAsync(settings);
                }
                catch (Exception ex)
                {
                    capturedException = ex;
                }
            }, listExtraOpt, listExportOpt, listExportFormatOpt);
            
            // Create update command
            var updateCommand = new Command("update", "Updates Tinycity, downloading the latest release from Github.");
            
            var updateExtraOpt = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            updateCommand.AddOption(updateExtraOpt);
            
            var updateCommandInstance = new UpdateCommand(serviceProvider);
            updateCommand.SetHandler(async (bool extra) =>
            {
                try
                {
                    var settings = new BaseSettings
                    {
                        Extra = extra
                    };
                    
                    interceptor.SetShowExtraInfo(settings.Extra);
                    await updateCommandInstance.ExecuteAsync(settings);
                }
                catch (Exception ex)
                {
                    capturedException = ex;
                }
            }, updateExtraOpt);
            
            // Create config command
            var configCommand = new Command("config", "Configure bookmark sources.");
            
            var configExtraOpt = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            var configAddMarkdownOpt = new Option<string?>(new[] { "-a", "--add-markdown-file" }, "Adds a markdown file to scan for links.");
            var configRemoveMarkdownOpt = new Option<string?>(new[] { "-r", "--remove-markdown-file" }, "Remove a markdown file from the config.");
            var configBrowserOpt = new Option<string?>(new[] { "-b", "--set-browser" }, "Set the browser type to search bookmarks from. Valid values are: chrome, opera, brave, edge.");
            var configHtmlBookmarkOpt = new Option<string?>(new[] { "-h", "--html-bookmark-file" }, "Sets a HTML bookmark file to scan for links.");
            var configBrowserBookmarkPathOpt = new Option<string?>(new[] { "-p", "--browser-bookmark-path" }, "Sets a Browser bookmark path, if the default doesn't exist.");
            
            configCommand.AddOption(configExtraOpt);
            configCommand.AddOption(configAddMarkdownOpt);
            configCommand.AddOption(configRemoveMarkdownOpt);
            configCommand.AddOption(configBrowserOpt);
            configCommand.AddOption(configHtmlBookmarkOpt);
            configCommand.AddOption(configBrowserBookmarkPathOpt);
            
            var configCommandInstance = new ConfigCommand(serviceProvider);
            configCommand.SetHandler(async (bool extra, string? addMarkdownFile, string? removeMarkdownFile, string? browser, string? htmlBookmarkFile, string? browserBookmarkPath) =>
            {
                try
                {
                    var settings = new ConfigCommandSettings
                    {
                        Extra = extra,
                        AddMarkdownFile = addMarkdownFile,
                        RemoveMarkdownFile = removeMarkdownFile,
                        Browser = browser,
                        HtmlBookmarkFile = htmlBookmarkFile,
                        BrowserBookmarkPath = browserBookmarkPath
                    };
                    
                    interceptor.SetShowExtraInfo(settings.Extra);
                    await configCommandInstance.ExecuteAsync(settings);
                }
                catch (Exception ex)
                {
                    capturedException = ex;
                }
            }, configExtraOpt, configAddMarkdownOpt, configRemoveMarkdownOpt, configBrowserOpt, configHtmlBookmarkOpt, configBrowserBookmarkPathOpt);

            rootCommand.AddCommand(searchCommand);
            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(updateCommand);
            rootCommand.AddCommand(configCommand);

            int result = await rootCommand.InvokeAsync(args);
            
            stopWatch.Stop();
            interceptor.ShowOutput(stopWatch, capturedException);
            
            return result;
        }

        static string GetVersion()
        {
            // Generated by GitVersion in its msbuild task, from the Git tag.
            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.1";
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

            return services;
        }

        static void EnsureDownloadedBackupIsRemoved()
        {
            string backupFilename = $"{Environment.ProcessPath}.bak";
            if (Path.Exists(backupFilename))
            {
                File.Delete(backupFilename);
            }
        }
    }
}