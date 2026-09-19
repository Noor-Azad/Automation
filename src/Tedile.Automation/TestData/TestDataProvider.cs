namespace Tedile.Automation.TestData;

public static class TestDataProvider
{
    public static IEnumerable<object[]> PublicLegalPages()
    {
        yield return new object[] { "/terms", "Terms of Service" };
        yield return new object[] { "/privacy", "Privacy Policy" };
        yield return new object[] { "/provider-nda", "Non-Disclosure Agreement" };
    }

    public static IEnumerable<object[]> InvalidCustomerPhones()
    {
        yield return new object[] { "12345" };
        yield return new object[] { "abcdefghij" };
    }
}
