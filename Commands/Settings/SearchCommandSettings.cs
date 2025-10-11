using System.CommandLine;
using System.CommandLine.Binding;

namespace TinyCity.Commands.Settings
{
    public class SearchCommandSettings : BaseSettings<SearchCommandSettings>
    {
        public bool Launch { get; set; }
        public bool SearchUrls { get; set; }
        public string Query { get; set; } = string.Empty;
        public bool Export { get; set; }
        public string ExportFormat { get; set; } = "- [{name}]({url}) ({urlhost})";

        private readonly Argument<string> _queryArgument;
        private readonly Option<bool> _extraOption;
        private readonly Option<bool> _launchOption;
        private readonly Option<bool> _searchUrlsOption;
        private readonly Option<bool> _exportOption;
        private readonly Option<string> _exportFormatOption;

        public SearchCommandSettings()
        {
            _queryArgument = new Argument<string>("query", "The search term to look for in bookmarks. Enclose your search inside quotes, e.g. \"my search words\"");
            _extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            _launchOption = new Option<bool>(new[] { "-l", "--launch" }, "Launch the first bookmark found in your default browser. If no bookmarks are found, nothing will happen.");
            _searchUrlsOption = new Option<bool>(new[] { "-u", "--urls" }, "Also search bookmark urls.");
            _exportOption = new Option<bool>(new[] { "-e", "--export" }, "Exports the results as 'exported-bookmarks.md' to the same directory as tinycity.");
            _exportFormatOption = new Option<string>("--export-format", () => "- [{name}]({url}) ({urlhost})", "When exporting, sets the format of each link");
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

        internal void AddOptionsToCommand(Command command)
        {
            command.AddArgument(_queryArgument);
            command.AddOption(_extraOption);
            command.AddOption(_launchOption);
            command.AddOption(_searchUrlsOption);
            command.AddOption(_exportOption);
            command.AddOption(_exportFormatOption);
        }
    }
}