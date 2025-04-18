namespace PreLaunchTaskr.Common.Utils;

public class EmptyList
{
    public static List<T> Of<T>() => new List<T>(0);
}
