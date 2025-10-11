using System.Diagnostics;
using Spectre.Console;
using TinyCity.Commands;

namespace TinyCity
{
    public class ExtraInfoInterceptor
    {
        private bool _showExtraInfo;

        public ExtraInfoInterceptor()
        {
            _showExtraInfo = false;
        }

        public void SetShowExtraInfo(bool showExtraInfo)
        {
            _showExtraInfo = showExtraInfo;
        }

        public void ShowOutput(Stopwatch stopwatch, Exception? ex)
        {
            if (_showExtraInfo)
            {
                if (ex != null)
                {
                    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                }

                AnsiConsole.MarkupLine($"[italic skyblue1]Took {stopwatch.ElapsedMilliseconds}ms to complete.[/]");
            }
            else
            {
                if (ex != null)
                {
                    AnsiConsole.MarkupLine($"{ex.Message} Use --help for usage.");
                }
            }
        }
    }
}