using System.Text;

namespace PreLaunchTaskr.Core.Extensions;

public static class ArrayExtension
{
    public static string ToString<T>(this T[] array, char separator) where T : notnull
    {
        //StringBuilder stringBuilder = new StringBuilder(array[0].ToString());
        //for (int i = 1; i < array.Length; i++)
        //{
        //    stringBuilder.Append(separator).Append(array[i].ToString());
        //}
        //return stringBuilder.ToString();
        return new StringBuilder().AppendJoin<T>(separator, array).ToString();
    }

    public static string ToString<T>(this T[] array, char separator, char beforeEach, char afterEach) where T : notnull
    {
        StringBuilder stringBuilder = new StringBuilder().Append(beforeEach).Append(array[0].ToString()).Append(afterEach);
        for (int i = 1; i < array.Length; i++)
        {
            stringBuilder.Append(separator).Append(beforeEach).Append(array[i].ToString()).Append(afterEach);
        }
        return stringBuilder.ToString();
    }
}
