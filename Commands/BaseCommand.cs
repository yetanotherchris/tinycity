using TinyCity.Commands.Settings;

namespace TinyCity.Commands
{
    public abstract class BaseCommand<TSettings>
    {
        protected readonly IServiceProvider _serviceProvider;
        
        protected BaseCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public abstract Task<int> ExecuteAsync(TSettings settings);
    }
}
