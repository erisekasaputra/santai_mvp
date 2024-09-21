using System.Text.RegularExpressions;

namespace Ordering.Domain.SeedWork;

public static partial class StringSplitter
{  
    [GeneratedRegex(@"(?<!^)(?=[A-Z])")]
    private static partial Regex RegexSplitByUpperCase();
    public static string SplitByUpperCase(this string input)
    { 
        string result = RegexSplitByUpperCase().Replace(input, " ");
        return result;
    }

}
