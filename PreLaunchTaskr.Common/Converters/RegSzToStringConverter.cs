using System.Text;

namespace PreLaunchTaskr.Common.Converters;

public class RegSzToStringConverter
{
    public static string Convert(string regsz)
    {
        if (string.IsNullOrWhiteSpace(regsz))
            return regsz;

        StringBuilder result = new();
        for (int i = 0; i < regsz.Length; i++)
        {
            if (regsz[i] == '\0')
                break;

            result.Append(regsz[i]);
        }
        return result.ToString();
    }

    public static string ConvertAndTrimQuotation(string regsz)
    {
        if (string.IsNullOrWhiteSpace(regsz))
            return regsz;

        StringBuilder result = new();
        for (int i = 0; i < regsz.Length; i++)
        {
            if (regsz[i] == '\0')
                break;

            result.Append(regsz[i]);
        }
        if (result[0] == '\"' && result[^1] == '\"')
        {
            result.Remove(0, 1).Remove(result.Length - 1, 1);
        }
        return result.ToString();
    }
}
