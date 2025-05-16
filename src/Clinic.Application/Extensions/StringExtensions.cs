namespace Clinic.Application.Extensions;

public static class StringExtensions
{
    public static bool HasSameValue(this string a, string b,
        StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
    {
        return string.Equals(b, a, stringComparison);
    }
}