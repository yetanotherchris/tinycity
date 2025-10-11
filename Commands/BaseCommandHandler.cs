using System.CommandLine;
using TinyCity.Commands.Settings;

namespace TinyCity.Commands
{
    public abstract class BaseCommandHandler<TSettings>
    {
        public abstract Task<int> ExecuteAsync(TSettings settings);

        public abstract Command CreateCommand(ExtraArgumentHandler extraArgumentHandler);
    }
}
