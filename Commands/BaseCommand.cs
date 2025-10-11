using TinyCity.Commands.Settings;

namespace TinyCity.Commands
{
    public abstract class BaseCommand<TSettings>
    {
        public abstract Task<int> ExecuteAsync(TSettings settings);
    }
}
