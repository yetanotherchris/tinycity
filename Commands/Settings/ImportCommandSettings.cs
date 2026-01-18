using System.CommandLine;
using System.CommandLine.Binding;

namespace TinyCity.Commands.Settings
{
    public class ImportCommandSettings : BaseSettings<ImportCommandSettings>
    {
        public string? Target { get; set; }
        public string? Directory { get; set; }

        private readonly Option<bool> _extraOption;
        private readonly Option<string?> _targetOption;
        private readonly Option<string?> _directoryOption;

        public ImportCommandSettings()
        {
            _extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            _targetOption = new Option<string?>(new[] { "--target" }, "Bookmark target to import to: chrome, brave, edge, opera, markdown, html, or all.");
            _directoryOption = new Option<string?>(new[] { "-d", "--directory" }, "Local directory path for import.");
        }

        protected override ImportCommandSettings GetBoundValue(BindingContext bindingContext)
        {
            return new ImportCommandSettings
            {
                Extra = bindingContext.ParseResult.GetValueForOption(_extraOption),
                Target = bindingContext.ParseResult.GetValueForOption(_targetOption),
                Directory = bindingContext.ParseResult.GetValueForOption(_directoryOption)
            };
        }

        internal void AddOptionsToCommand(Command command)
        {
            command.AddOption(_extraOption);
            command.AddOption(_targetOption);
            command.AddOption(_directoryOption);
        }
    }
}
