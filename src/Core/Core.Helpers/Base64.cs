
using System;
using System.Text;

namespace Core.Helpers;

public static class Base64
{
    public static string? ConvertToString(this string? base64Encoded)
    {
        if (string.IsNullOrWhiteSpace(base64Encoded))
            return null;

        byte[] data = Convert.FromBase64String(base64Encoded);
        string decodedString = Encoding.UTF8.GetString(data);
        return decodedString;
    }
}
