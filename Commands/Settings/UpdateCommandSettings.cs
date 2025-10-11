using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Binding;
using TinyCity.Commands.Settings;

namespace TinyCity.Commands.Settings
{
    public class UpdateCommandSettings : BaseSettings<UpdateCommandSettings>
    {
        private readonly Option<bool> _extraOption;

        public UpdateCommandSettings()
        {
            _extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
        }

        public Option<bool> ExtraOption => _extraOption;

        protected override UpdateCommandSettings GetBoundValue(BindingContext bindingContext)
        {
            return new UpdateCommandSettings
            {
                Extra = bindingContext.ParseResult.GetValueForOption(_extraOption)
            };
        }

        internal void ConfigureCommand(Command command)
        {
            command.AddOption(ExtraOption);
        }
    }
}