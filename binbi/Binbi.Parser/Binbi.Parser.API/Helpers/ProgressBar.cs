using System.Text;

namespace Binbi.Parser.API.Helpers;

internal class ProgressBar
{
    private readonly int _total;
    private int _current;
    private readonly int _width;
    private readonly object _lock = new();

    internal ProgressBar(int total, int width = 50)
    {
        _total = total;
        _current = 0;
        _width = width;
    }

    internal void Report(int value)
    {
        lock (_lock)
        {
            _current = value;
            Draw();
        }
    }

    private void Draw()
    {
        var progress = (int)(_current / (double)_total * _width);
        var percentage = (int)(_current / (double)_total * 100);

        var progressBar = new StringBuilder();
        progressBar.Append('[');
        progressBar.Append(new string('#', progress));
        progressBar.Append(new string('-', _width - progress));
        progressBar.Append(']');
        progressBar.Append($" {percentage}%");

        var originalPos = Console.GetCursorPosition();

        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(progressBar + new string(' ', Console.WindowWidth - progressBar.Length));

        Console.SetCursorPosition(originalPos.Left, originalPos.Top);
    }
}