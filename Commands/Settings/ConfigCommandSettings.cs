using System.CommandLine;
using System.CommandLine.Binding;

namespace TinyCity.Commands.Settings
{
    public class ConfigCommandSettings : BaseSettings<ConfigCommandSettings>
    {
        public string? AddSource { get; set; }
        public string? RemoveSource { get; set; }

        private readonly Option<bool> _extraOption;
        private readonly Option<string?> _addSourceOption;
        private readonly Option<string?> _removeSourceOption;

        public ConfigCommandSettings()
        {
            _extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            _addSourceOption = new Option<string?>("--add-source", "Adds a bookmark source. Can be a browser name (chrome, brave, edge, opera) or a file path (.md, .html, or browser bookmark file).");
            _removeSourceOption = new Option<string?>("--remove-source", "Removes a bookmark source. Can be a browser name or file path.");
        }

        protected override ConfigCommandSettings GetBoundValue(BindingContext bindingContext)
        {
            return new ConfigCommandSettings
            {
                Extra = bindingContext.ParseResult.GetValueForOption(_extraOption),
                AddSource = bindingContext.ParseResult.GetValueForOption(_addSourceOption),
                RemoveSource = bindingContext.ParseResult.GetValueForOption(_removeSourceOption)
            };
        }

        internal void AddOptionsToCommand(Command command)
        {
            command.AddOption(_extraOption);
            command.AddOption(_addSourceOption);
            command.AddOption(_removeSourceOption);
        }
    }
}