using System.Diagnostics;
using Spectre.Console;

namespace TinyCity.Commands
{
    public class ExtraArgumentHandler
    {
        private bool _showExtraInfo;

        public ExtraArgumentHandler()
        {
            _showExtraInfo = false;
        }

        public void SetShowExtraInfo(bool showExtraInfo)
        {
            _showExtraInfo = showExtraInfo;
        }

        public void ShowOutput(Stopwatch stopwatch)
        {
            if (_showExtraInfo)
            {
                AnsiConsole.MarkupLine($"[italic skyblue1]Took {stopwatch.ElapsedMilliseconds}ms to complete.[/]");
            }
        }
    }
}