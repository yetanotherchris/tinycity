using System.CommandLine;
using System.CommandLine.Binding;

namespace TinyCity.Commands.Settings
{
    public class UpdateCommandSettings : BaseSettings<UpdateCommandSettings>
    {
        private readonly Option<bool> _extraOption;

        public UpdateCommandSettings()
        {
            _extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
        }

        protected override UpdateCommandSettings GetBoundValue(BindingContext bindingContext)
        {
            return new UpdateCommandSettings
            {
                Extra = bindingContext.ParseResult.GetValueForOption(_extraOption)
            };
        }

        internal void AddOptionsToCommand(Command command)
        {
            command.AddOption(_extraOption);
        }
    }
}