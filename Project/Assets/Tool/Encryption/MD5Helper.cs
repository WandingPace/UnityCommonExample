using UnityEngine;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

public static class MD5Helper
{
    public static string GetHash(string text)
    {
        byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes(text.ToCharArray());
        MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(bytes);
        StringBuilder sb = new StringBuilder(128);
        for (int i = 0; i < hash.Length; ++i)
        {
            sb.Append(hash[i].ToString("x2"));
        }
        return sb.ToString();
    }

    public static string GetHash(byte[] bytes)
    {
        MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(bytes);
        StringBuilder sb = new StringBuilder(128);
        for (int i = 0; i < hash.Length; ++i)
        {
            sb.Append(hash[i].ToString("x2"));
        }
        return sb.ToString();
    }
}
