using System.Net;
using System.Security.Cryptography;
using System.Text;

public static class VnpayConfig
{
    public static string vnp_TmnCode = "WSDB6HN3";
    public static string vnp_HashSecret = "PW6B1YNBG3242VWMSCSHKZHMF02O4YG7";
    public static string vnp_Returnurl = "/vnpay-return";
    public static string vnp_PayUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";

    public static string GetRandomNumber(int length)
    {
        var random = new Random();
        var sb = new StringBuilder();
        for (int i = 0; i < length; i++)
            sb.Append(random.Next(10));
        return sb.ToString();
    }

    public static string GetIpAddress(HttpRequest request)
    {
        return request.Headers.ContainsKey("X-Forwarded-For")
            ? request.Headers["X-Forwarded-For"].FirstOrDefault()
            : request.HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    public static string HmacSHA512(string key, string inputData)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(inputBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    public static string BuildQueryString(SortedDictionary<string, string> data)
    {
        return string.Join("&", data.Select(kv =>
            $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));
    }

    public static string BuildDataToHash(SortedDictionary<string, string> data)
    {
        return string.Join("&", data.Select(kv =>
            $"{kv.Key}={kv.Value}"));
    }

    public static string HashAllFields(SortedDictionary<string, string> fields)
    {
        string rawData = BuildDataToHash(fields);
        return HmacSHA512(vnp_HashSecret, rawData);
    }
}
