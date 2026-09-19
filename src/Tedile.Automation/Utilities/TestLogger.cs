namespace Tedile.Automation.Utilities;

public sealed class TestLogger
{
    private readonly Action<string> _write;

    public TestLogger(Action<string> write) => _write = write;

    public void Info(string message) => Write("INFO", message);
    public void Step(string message) => Write("STEP", message);
    public void Warn(string message) => Write("WARN", message);

    private void Write(string level, string message) =>
        _write($"[{DateTime.UtcNow:O}] [{level}] {message}");
}
