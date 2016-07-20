using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Security.Cryptography;

public class AESHelper
{
    public static byte[] CreateKey(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException("key");
        byte[] bkey = new byte[32];
        Array.Copy(System.Text.UTF8Encoding.UTF8.GetBytes(key.PadRight(bkey.Length)), bkey, bkey.Length);
        return bkey;
    }
    public static byte[] CreateVector(string vector)
    {
        if (string.IsNullOrEmpty(vector))
            throw new ArgumentNullException("vector");
        byte[] bvector = new byte[16];
        Array.Copy(System.Text.UTF8Encoding.UTF8.GetBytes(vector.PadRight(bvector.Length)), bvector, bvector.Length);
        return bvector;
    }
    public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
    {
        if (data == null || data.Length < 0)
            throw new ArgumentNullException("data");
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (iv == null || iv.Length <= 0)
            throw new ArgumentNullException("iv");

        byte[] encrypteddata = null;
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (BinaryWriter sw = new BinaryWriter(cs))
                        {
                            sw.Write(data);
                        }
                    }
                    encrypteddata = ms.ToArray();
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }

        return encrypteddata;
    }
    public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
    {
        if (data == null || data.Length < 0)
            throw new ArgumentNullException("data");
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (iv == null || iv.Length <= 0)
            throw new ArgumentNullException("iv");

        byte[] decrypteddata = null;
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor();
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (BinaryReader br = new BinaryReader(cs))
                        {
                            using (MemoryStream bms = new MemoryStream())
                            {
                                byte[] buffer = new byte[1024];
                                int readbyte = 0;
                                while (((readbyte = br.Read(buffer, 0, buffer.Length)) != 0))
                                {
                                    bms.Write(buffer, 0, readbyte);
                                }
                                decrypteddata = bms.ToArray();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }

        return decrypteddata;
    }
    public static string Encrypt(string text, byte[] key, byte[] iv)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentNullException("text");
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (iv == null || iv.Length <= 0)
            throw new ArgumentNullException("iv");

        byte[] encrypteddata = null;
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(text);
                        }
                    }
                    encrypteddata = ms.ToArray();
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }

        return Convert.ToBase64String(encrypteddata);
    }
    public static string Decrypt(string text, byte[] key, byte[] iv)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentNullException("data");
        if (key == null || key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (iv == null || iv.Length <= 0)
            throw new ArgumentNullException("iv");

        byte[] data = Convert.FromBase64String(text);
        string decryptetext = null;
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor();
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader br = new StreamReader(cs))
                        {
                            decryptetext = br.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw e;
        }

        return decryptetext;
    }
}

