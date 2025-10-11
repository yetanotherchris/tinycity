using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Binding;
using TinyCity.Commands.Settings;

namespace TinyCity.Commands.Settings
{
    public class UpdateCommandSettings : BaseSettings
    {
        public static Command CreateCommand(IServiceProvider serviceProvider, ExtraInfoInterceptor interceptor, Action<Exception> onException)
        {
            var command = new Command("update", "Updates Tinycity, downloading the latest release from Github.");

            var extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            command.AddOption(extraOption);

            var settingsBinder = new UpdateCommandSettingsBinder(extraOption);

            command.SetHandler(async (UpdateCommandSettings settings) =>
            {
                try
                {
                    var updateCommandInstance = serviceProvider.GetRequiredService<UpdateCommand>();
                    interceptor.SetShowExtraInfo(settings.Extra);
                    await updateCommandInstance.ExecuteAsync(settings);
                }
                catch (Exception ex)
                {
                    onException(ex);
                }
            }, settingsBinder);

            return command;
        }

        private class UpdateCommandSettingsBinder : BinderBase<UpdateCommandSettings>
        {
            private readonly Option<bool> _extraOption;

            public UpdateCommandSettingsBinder(Option<bool> extraOption)
            {
                _extraOption = extraOption;
            }

            protected override UpdateCommandSettings GetBoundValue(BindingContext bindingContext)
            {
                return new UpdateCommandSettings
                {
                    Extra = bindingContext.ParseResult.GetValueForOption(_extraOption)
                };
            }
        }
    }
}