using UnityEngine;
using System;
using System.Collections;

public class LoginRecord : ILocalSerializable
{
    public string channel = "";//class LoginChannel strings.
    public string internalId = "";//internal account id.
    public string userid = ""; //channel userid.
    public string username = "";//channel username.
    public string password = "";//channel password can be empty, if channel is not playfab.
    public string token = ""; //login session or token string. can be empty.
    public DateTime date = DateTime.Now;
    public string channeldata = "";//extra data for channel. json.

    public void Serialize(LocalSerializationContext context)
    {
        context.writer.Write(channel ?? "");
        context.writer.Write(internalId ?? "");
        context.writer.Write(userid ?? "");
        context.writer.Write(username ?? "");
        context.writer.Write(password ?? "");
        context.writer.Write(token ?? "");
        context.writer.Write(date.ToString());
        context.writer.Write(channeldata ?? "");
    }

    public void Deserialize(LocalSerializationContext context)
    {
        channel = context.reader.ReadString();
        internalId = context.reader.ReadString();
        userid = context.reader.ReadString();
        username = context.reader.ReadString();
        password = context.reader.ReadString();
        token = context.reader.ReadString();
        string datestr = context.reader.ReadString();
        date = DateTime.Parse(datestr);
        channeldata = context.reader.ReadString();
    }
}

public class LoginRecordManager
{
    #region Static
    private static readonly LoginRecordManager _instance = new LoginRecordManager();
    public static LoginRecordManager instance
    {
        get
        {
            return _instance;
        }
    }
    private LoginRecordManager()
    {
        _secretKey = AESHelper.CreateKey(CRYPTO_KEY);
    }
    #endregion

    private const string FILE_NAME = "loginrecord";
    private const long EXPIRE_TIME = -1;
    private const string CRYPTO_KEY = "www.virtuos.com.key";
    
    private byte[] _secretKey = null;
    public byte[] secretKey
    {
        get
        {
            if (_secretKey == null)
            {
                _secretKey = AESHelper.CreateKey(CRYPTO_KEY);
            }
            return _secretKey;
        }
    }

    public string GetFileName(string filename)
    {
        return string.Format("{0}.history", MD5Helper.GetHash(filename));
    }

    public void Save(LoginRecord record)
    {
        if (record == null)
            return;

        string filename = GetFileName(FILE_NAME);
        LocalDataManager.Save<LoginRecord>(record, filename, true, secretKey);
    }

    public LoginRecord Load()
    {
        string filename = GetFileName(FILE_NAME);
        try
        {
            return LocalDataManager.Load<LoginRecord>(filename, secretKey);    
        }
        catch
        {
            Debug.Log("Record Parse Error");
            return null;
        }
    }

    public void Delete()
    {
        string filename = GetFileName(FILE_NAME);
        LocalDataManager.Delete(filename);
    }
}
