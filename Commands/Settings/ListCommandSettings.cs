using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Binding;
using TinyCity.Commands.Settings;

namespace TinyCity.Commands.Settings
{
    public class ListCommandSettings : BaseSettings
    {
        public bool Export { get; set; }
        public string ExportFormat { get; set; } = "- [{name}]({url}) ({urlhost})";

        public static Command CreateCommand(IServiceProvider serviceProvider, ExtraInfoInterceptor interceptor, Action<Exception> onException)
        {
            var command = new Command("ls", "List all bookmarks.");
            command.AddAlias("list");

            var extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            var exportOption = new Option<bool>(new[] { "-e", "--export" }, "Exports the results as 'exported-bookmarks.md' to the same directory as tinycity.");
            var exportFormatOption = new Option<string>("--export-format", () => "- [{name}]({url}) ({urlhost})", "When exporting, sets the format of each link");

            command.AddOption(extraOption);
            command.AddOption(exportOption);
            command.AddOption(exportFormatOption);

            var settingsBinder = new ListCommandSettingsBinder(
                extraOption,
                exportOption,
                exportFormatOption);

            command.SetHandler(async (ListCommandSettings settings) =>
            {
                try
                {
                    var listCommandInstance = serviceProvider.GetRequiredService<ListCommand>();
                    interceptor.SetShowExtraInfo(settings.Extra);
                    await listCommandInstance.ExecuteAsync(settings);
                }
                catch (Exception ex)
                {
                    onException(ex);
                }
            }, settingsBinder);

            return command;
        }

        private class ListCommandSettingsBinder : BinderBase<ListCommandSettings>
        {
            private readonly Option<bool> _extraOption;
            private readonly Option<bool> _exportOption;
            private readonly Option<string> _exportFormatOption;

            public ListCommandSettingsBinder(
                Option<bool> extraOption,
                Option<bool> exportOption,
                Option<string> exportFormatOption)
            {
                _extraOption = extraOption;
                _exportOption = exportOption;
                _exportFormatOption = exportFormatOption;
            }

            protected override ListCommandSettings GetBoundValue(BindingContext bindingContext)
            {
                return new ListCommandSettings
                {
                    Extra = bindingContext.ParseResult.GetValueForOption(_extraOption),
                    Export = bindingContext.ParseResult.GetValueForOption(_exportOption),
                    ExportFormat = bindingContext.ParseResult.GetValueForOption(_exportFormatOption) ?? "- [{name}]({url}) ({urlhost})"
                };
            }
        }
    }
}