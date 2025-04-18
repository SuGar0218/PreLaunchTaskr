namespace PreLaunchTaskr.Common.Helpers;

public class StringHelper
{
    public static string TrimQuotes(string text)
    {
        text = text.Trim();
        if ((text.StartsWith('\"') || text.StartsWith('\'')) && (text.EndsWith('\"') || text.EndsWith('\'')))
        {
            return text.Substring(1, text.Length - 2);
        }
        return text;
    }

    public static string GetFirstToken(string text)
    {
        text = text.Trim();

        if (text[0] == '\"' || text[0] == '\'')
        {
            int i;
            for (i = 1; i < text.Length; i++)
            {
                if (text[i] == text[0])
                    break;
            }
            return text.Substring(1, i - 1);
        }

        int indexOfFirstSpace = text.IndexOf(' ');
        return indexOfFirstSpace > 0 ? text.Substring(0, indexOfFirstSpace) : text;
    }
}
