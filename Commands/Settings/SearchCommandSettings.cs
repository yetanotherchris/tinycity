using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Binding;
using TinyCity.Commands.Settings;

namespace TinyCity.Commands.Settings
{
    public class SearchCommandSettings : BaseSettings
    {
        public bool Launch { get; set; }
        public bool SearchUrls { get; set; }
        public string Query { get; set; } = string.Empty;
        public bool Export { get; set; }
        public string ExportFormat { get; set; } = "- [{name}]({url}) ({urlhost})";

        public static Command CreateCommand(IServiceProvider serviceProvider, ExtraInfoInterceptor interceptor, Action<Exception> onException)
        {
            var command = new Command("search", "Search the bookmarks.");
            command.AddAlias("q");

            var queryArg = new Argument<string>("query", "The search term to look for in bookmarks. Enclose your search inside quotes, e.g. \"my search words\"");
            var extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            var launchOption = new Option<bool>(new[] { "-l", "--launch" }, "Launch the first bookmark found in your default browser. If no bookmarks are found, nothing will happen.");
            var searchUrlsOption = new Option<bool>(new[] { "-u", "--urls" }, "Also search bookmark urls.");
            var exportOption = new Option<bool>(new[] { "-e", "--export" }, "Exports the results as 'exported-bookmarks.md' to the same directory as tinycity.");
            var exportFormatOption = new Option<string>("--export-format", () => "- [{name}]({url}) ({urlhost})", "When exporting, sets the format of each link");

            command.AddArgument(queryArg);
            command.AddOption(extraOption);
            command.AddOption(launchOption);
            command.AddOption(searchUrlsOption);
            command.AddOption(exportOption);
            command.AddOption(exportFormatOption);

            var settingsBinder = new SearchCommandSettingsBinder(
                queryArg,
                extraOption,
                launchOption,
                searchUrlsOption,
                exportOption,
                exportFormatOption);

            command.SetHandler(async (SearchCommandSettings settings) =>
            {
                try
                {
                    var searchCommandInstance = serviceProvider.GetRequiredService<SearchCommand>();
                    interceptor.SetShowExtraInfo(settings.Extra);
                    await searchCommandInstance.ExecuteAsync(settings);
                }
                catch (Exception ex)
                {
                    onException(ex);
                }
            }, settingsBinder);

            return command;
        }

        private class SearchCommandSettingsBinder : BinderBase<SearchCommandSettings>
        {
            private readonly Argument<string> _queryArgument;
            private readonly Option<bool> _extraOption;
            private readonly Option<bool> _launchOption;
            private readonly Option<bool> _searchUrlsOption;
            private readonly Option<bool> _exportOption;
            private readonly Option<string> _exportFormatOption;

            public SearchCommandSettingsBinder(
                Argument<string> queryArgument,
                Option<bool> extraOption,
                Option<bool> launchOption,
                Option<bool> searchUrlsOption,
                Option<bool> exportOption,
                Option<string> exportFormatOption)
            {
                _queryArgument = queryArgument;
                _extraOption = extraOption;
                _launchOption = launchOption;
                _searchUrlsOption = searchUrlsOption;
                _exportOption = exportOption;
                _exportFormatOption = exportFormatOption;
            }

            protected override SearchCommandSettings GetBoundValue(BindingContext bindingContext)
            {
                return new SearchCommandSettings
                {
                    Query = bindingContext.ParseResult.GetValueForArgument(_queryArgument) ?? string.Empty,
                    Extra = bindingContext.ParseResult.GetValueForOption(_extraOption),
                    Launch = bindingContext.ParseResult.GetValueForOption(_launchOption),
                    SearchUrls = bindingContext.ParseResult.GetValueForOption(_searchUrlsOption),
                    Export = bindingContext.ParseResult.GetValueForOption(_exportOption),
                    ExportFormat = bindingContext.ParseResult.GetValueForOption(_exportFormatOption) ?? "- [{name}]({url}) ({urlhost})"
                };
            }
        }
    }
}