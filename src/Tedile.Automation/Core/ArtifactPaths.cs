namespace Tedile.Automation.Core;

public static class ArtifactPaths
{
    public static string EnsureRoot()
    {
        var root = Path.Combine(Directory.GetCurrentDirectory(), "artifacts");
        Directory.CreateDirectory(root);
        return root;
    }

    public static string Trace(string testName)
    {
        var folder = Path.Combine(EnsureRoot(), "traces");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, $"{SafeName(testName)}-{DateTime.UtcNow:yyyyMMdd-HHmmssfff}.zip");
    }

    public static string Screenshot(string testName)
    {
        var folder = Path.Combine(EnsureRoot(), "screenshots");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, $"{SafeName(testName)}-{DateTime.UtcNow:yyyyMMdd-HHmmssfff}.png");
    }

    private static string SafeName(string value) =>
        string.Concat(value.Select(c => char.IsLetterOrDigit(c) || c is '-' or '_' ? c : '_'));
}
