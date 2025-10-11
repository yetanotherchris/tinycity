using System.CommandLine;
using System.CommandLine.Binding;

namespace TinyCity.Commands.Settings
{
    public class ConfigCommandSettings : BaseSettings<ConfigCommandSettings>
    {
        public string? AddMarkdownFile { get; set; }
        public string? RemoveMarkdownFile { get; set; }
        public string? Browser { get; set; }
        public string? HtmlBookmarkFile { get; set; }
        public string? BrowserBookmarkPath { get; set; }

        private readonly Option<bool> _extraOption;
        private readonly Option<string?> _addMarkdownOption;
        private readonly Option<string?> _removeMarkdownOption;
        private readonly Option<string?> _browserOption;
        private readonly Option<string?> _htmlBookmarkOption;
        private readonly Option<string?> _browserBookmarkPathOption;

        public ConfigCommandSettings()
        {
            _extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            _addMarkdownOption = new Option<string?>(new[] { "-a", "--add-markdown-file" }, "Adds a markdown file to scan for links.");
            _removeMarkdownOption = new Option<string?>(new[] { "-r", "--remove-markdown-file" }, "Remove a markdown file from the config.");
            _browserOption = new Option<string?>(new[] { "-b", "--set-browser" }, "Set the browser type to search bookmarks from. Valid values are: chrome, opera, brave, edge.");
            _htmlBookmarkOption = new Option<string?>(new[] { "-h", "--html-bookmark-file" }, "Sets a HTML bookmark file to scan for links.");
            _browserBookmarkPathOption = new Option<string?>(new[] { "-p", "--browser-bookmark-path" }, "Sets a Browser bookmark path, if the default doesn't exist.");
        }

        protected override ConfigCommandSettings GetBoundValue(BindingContext bindingContext)
        {
            return new ConfigCommandSettings
            {
                Extra = bindingContext.ParseResult.GetValueForOption(_extraOption),
                AddMarkdownFile = bindingContext.ParseResult.GetValueForOption(_addMarkdownOption),
                RemoveMarkdownFile = bindingContext.ParseResult.GetValueForOption(_removeMarkdownOption),
                Browser = bindingContext.ParseResult.GetValueForOption(_browserOption),
                HtmlBookmarkFile = bindingContext.ParseResult.GetValueForOption(_htmlBookmarkOption),
                BrowserBookmarkPath = bindingContext.ParseResult.GetValueForOption(_browserBookmarkPathOption)
            };
        }

        internal void AddOptionsToCommand(Command command)
        {
            command.AddOption(_extraOption);
            command.AddOption(_addMarkdownOption);
            command.AddOption(_removeMarkdownOption);
            command.AddOption(_browserOption);
            command.AddOption(_htmlBookmarkOption);
            command.AddOption(_browserBookmarkPathOption);
        }
    }
}