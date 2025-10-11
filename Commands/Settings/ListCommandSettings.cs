using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Binding;
using TinyCity.Commands.Settings;

namespace TinyCity.Commands.Settings
{
    public class ListCommandSettings : BaseSettings<ListCommandSettings>
    {
        public bool Export { get; set; }
        public string ExportFormat { get; set; } = "- [{name}]({url}) ({urlhost})";

        private readonly Option<bool> _extraOption;
        private readonly Option<bool> _exportOption;
        private readonly Option<string> _exportFormatOption;

        public ListCommandSettings()
        {
            _extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            _exportOption = new Option<bool>(new[] { "-e", "--export" }, "Exports the results as 'exported-bookmarks.md' to the same directory as tinycity.");
            _exportFormatOption = new Option<string>("--export-format", () => "- [{name}]({url}) ({urlhost})", "When exporting, sets the format of each link");
        }

        public Option<bool> ExtraOption => _extraOption;
        public Option<bool> ExportOption => _exportOption;
        public Option<string> ExportFormatOption => _exportFormatOption;

        protected override ListCommandSettings GetBoundValue(BindingContext bindingContext)
        {
            return new ListCommandSettings
            {
                Extra = bindingContext.ParseResult.GetValueForOption(_extraOption),
                Export = bindingContext.ParseResult.GetValueForOption(_exportOption),
                ExportFormat = bindingContext.ParseResult.GetValueForOption(_exportFormatOption) ?? "- [{name}]({url}) ({urlhost})"
            };
        }

        internal void ConfigureCommand(Command command)
        {
            command.AddOption(ExtraOption);
            command.AddOption(ExportOption);
            command.AddOption(ExportFormatOption);
        }
    }
}