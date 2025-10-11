using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Binding;
using TinyCity.Commands.Settings;

namespace TinyCity.Commands.Settings
{
    public class ConfigCommandSettings : BaseSettings
    {
        public string? AddMarkdownFile { get; set; }
        public string? RemoveMarkdownFile { get; set; }
        public string? Browser { get; set; }
        public string? HtmlBookmarkFile { get; set; }
        public string? BrowserBookmarkPath { get; set; }

        public static Command CreateCommand(IServiceProvider serviceProvider, ExtraInfoInterceptor interceptor, Action<Exception> onException)
        {
            var command = new Command("config", "Configure bookmark sources.");

            var extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            var addMarkdownOption = new Option<string?>(new[] { "-a", "--add-markdown-file" }, "Adds a markdown file to scan for links.");
            var removeMarkdownOption = new Option<string?>(new[] { "-r", "--remove-markdown-file" }, "Remove a markdown file from the config.");
            var browserOption = new Option<string?>(new[] { "-b", "--set-browser" }, "Set the browser type to search bookmarks from. Valid values are: chrome, opera, brave, edge.");
            var htmlBookmarkOption = new Option<string?>(new[] { "-h", "--html-bookmark-file" }, "Sets a HTML bookmark file to scan for links.");
            var browserBookmarkPathOption = new Option<string?>(new[] { "-p", "--browser-bookmark-path" }, "Sets a Browser bookmark path, if the default doesn't exist.");

            command.AddOption(extraOption);
            command.AddOption(addMarkdownOption);
            command.AddOption(removeMarkdownOption);
            command.AddOption(browserOption);
            command.AddOption(htmlBookmarkOption);
            command.AddOption(browserBookmarkPathOption);

            var settingsBinder = new ConfigCommandSettingsBinder(
                extraOption,
                addMarkdownOption,
                removeMarkdownOption,
                browserOption,
                htmlBookmarkOption,
                browserBookmarkPathOption);

            command.SetHandler(async (ConfigCommandSettings settings) =>
            {
                try
                {
                    var configCommandInstance = serviceProvider.GetRequiredService<ConfigCommand>();
                    interceptor.SetShowExtraInfo(settings.Extra);
                    await configCommandInstance.ExecuteAsync(settings);
                }
                catch (Exception ex)
                {
                    onException(ex);
                }
            }, settingsBinder);

            return command;
        }

        private class ConfigCommandSettingsBinder : BinderBase<ConfigCommandSettings>
        {
            private readonly Option<bool> _extraOption;
            private readonly Option<string?> _addMarkdownOption;
            private readonly Option<string?> _removeMarkdownOption;
            private readonly Option<string?> _browserOption;
            private readonly Option<string?> _htmlBookmarkOption;
            private readonly Option<string?> _browserBookmarkPathOption;

            public ConfigCommandSettingsBinder(
                Option<bool> extraOption,
                Option<string?> addMarkdownOption,
                Option<string?> removeMarkdownOption,
                Option<string?> browserOption,
                Option<string?> htmlBookmarkOption,
                Option<string?> browserBookmarkPathOption)
            {
                _extraOption = extraOption;
                _addMarkdownOption = addMarkdownOption;
                _removeMarkdownOption = removeMarkdownOption;
                _browserOption = browserOption;
                _htmlBookmarkOption = htmlBookmarkOption;
                _browserBookmarkPathOption = browserBookmarkPathOption;
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
        }
    }
}