using System.CommandLine;
using System.CommandLine.Binding;

namespace TinyCity.Commands.Settings
{
    public class ExportCommandSettings : BaseSettings<ExportCommandSettings>
    {
        public string? Source { get; set; }
        public string? Directory { get; set; }

        private readonly Option<bool> _extraOption;
        private readonly Option<string?> _sourceOption;
        private readonly Option<string?> _directoryOption;

        public ExportCommandSettings()
        {
            _extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            _sourceOption = new Option<string?>(new[] { "-s", "--source" }, "Bookmark source to export: chrome, brave, edge, opera, markdown, html, or all.");
            _directoryOption = new Option<string?>(new[] { "-d", "--directory" }, "Local directory path for export.");
        }

        protected override ExportCommandSettings GetBoundValue(BindingContext bindingContext)
        {
            return new ExportCommandSettings
            {
                Extra = bindingContext.ParseResult.GetValueForOption(_extraOption),
                Source = bindingContext.ParseResult.GetValueForOption(_sourceOption),
                Directory = bindingContext.ParseResult.GetValueForOption(_directoryOption)
            };
        }

        internal void AddOptionsToCommand(Command command)
        {
            command.AddOption(_extraOption);
            command.AddOption(_sourceOption);
            command.AddOption(_directoryOption);
        }
    }
}
