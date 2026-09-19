using System.Collections;

namespace Tedile.Automation.Tests;

public static class NUnitAssert
{
    public static void Equal<T>(T expected, T actual, string? message = null) =>
        NUnit.Framework.Assert.That(actual, Is.EqualTo(expected), message);

    public static void NotEqual<T>(T notExpected, T actual, string? message = null) =>
        NUnit.Framework.Assert.That(actual, Is.Not.EqualTo(notExpected), message);

    public static void True(bool condition, string? message = null) =>
        NUnit.Framework.Assert.That(condition, Is.True, message);

    public static void False(bool condition, string? message = null) =>
        NUnit.Framework.Assert.That(condition, Is.False, message);

    public static void NotNull(object? value, string? message = null) =>
        NUnit.Framework.Assert.That(value, Is.Not.Null, message);

    public static void Empty(IEnumerable values) =>
        NUnit.Framework.Assert.That(values, Is.Empty);

    public static void Contains(string expectedSubstring, string actualString) =>
        NUnit.Framework.Assert.That(actualString, Does.Contain(expectedSubstring));

    public static void Contains(
        string expectedSubstring,
        string actualString,
        StringComparison comparison) =>
        NUnit.Framework.Assert.That(
            actualString.Contains(expectedSubstring, comparison),
            Is.True,
            $"Expected string to contain '{expectedSubstring}'.");

    public static void DoesNotContain(string expectedSubstring, string actualString) =>
        NUnit.Framework.Assert.That(actualString, Does.Not.Contain(expectedSubstring));

    public static void DoesNotContain(
        string expectedSubstring,
        string actualString,
        StringComparison comparison) =>
        NUnit.Framework.Assert.That(
            actualString.Contains(expectedSubstring, comparison),
            Is.False,
            $"Expected string not to contain '{expectedSubstring}'.");

    public static void Contains<T>(IEnumerable<T> values, Predicate<T> predicate) =>
        NUnit.Framework.Assert.That(values.Any(value => predicate(value)), Is.True);

    public static void Contains<T>(T expected, IEnumerable<T> values) =>
        NUnit.Framework.Assert.That(values, Does.Contain(expected));

    public static void DoesNotContain<T>(T expected, IEnumerable<T> values) =>
        NUnit.Framework.Assert.That(values, Does.Not.Contain(expected));

    public static void DoesNotContain<T>(IEnumerable<T> values, Predicate<T> predicate) =>
        NUnit.Framework.Assert.That(values.Any(value => predicate(value)), Is.False);
}
