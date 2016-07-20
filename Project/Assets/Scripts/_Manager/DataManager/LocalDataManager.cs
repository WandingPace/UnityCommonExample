using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public enum LocalDataVersion
{
    VERSION_FIRST_PLAYABLE = 1,
    VERSION_ALPHA_1 = 2,
    VERSION_ALPHA_2 = 3,
    CURRENT = VERSION_ALPHA_2,
    ATLEAST = VERSION_ALPHA_2,
}

public interface ILocalSerializable
{
    void Serialize(LocalSerializationContext context);
    void Deserialize(LocalSerializationContext context);
}

public class LocalSerializationContext
{
    public BinaryWriter writer = null;
    public BinaryReader reader = null;
    public int version = 0;
}

public class LocalDataConfig
{

}

public static class LocalDataManager
{
    public static readonly byte[] cryptvector = new byte[16]
    {
        0x74, 0x65, 0x73, 0x74, 0x30, 0x30, 0x31, 0x2C, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x2C, 0x31
    };

    public static string localDataPath
    {
        get
        {
            return Application.persistentDataPath;
        }
    }

    public static void Save<T>(T data, string filename, bool encrypt, byte[] cryptkey) where T : ILocalSerializable, new()
    {
        //check valid.
        if (data == null)
            throw new ArgumentNullException("data");
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException(filename);
        if (encrypt && cryptkey == null || cryptkey.Length != 32)
            throw new ArgumentException("cryptkey==null or cryptkey.Length != 32");

        //get filepath;
        int version = (int)LocalDataVersion.CURRENT;
        byte[] lawdata = Serialize<T>(data, version);
        string filepath = Path.Combine(localDataPath, filename);

        byte[] finaldata = lawdata;

        //if encrypt data if need.
        string secret = "";
        if (encrypt)
        {
            finaldata = AESHelper.Encrypt(lawdata, cryptkey, cryptvector);
            secret = MD5Helper.GetHash(cryptkey);
        }

        //write file.
        using (FileStream fs = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write))
        {
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(version);//version.
                bw.Write(encrypt);//is encrypted
                if (encrypt)
                {
                    bw.Write(secret);//add secret.
                }
                bw.Write(finaldata);
            }
        }
    }

    public static T Load<T>(string filename, byte[] cryptkey) where T : ILocalSerializable, new()
    {
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentNullException("instance");

        string filepath = Path.Combine(localDataPath, filename);
        if (!File.Exists(filepath))
            return default(T);

        //load file data to the memory stream.
        MemoryStream ms = new MemoryStream();
        using (FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.Read))
        {
            using (BinaryReader br = new BinaryReader(fs))
            {
                byte[] buffer = new byte[1024];
                int readbyte = 0;
                while (((readbyte = br.Read(buffer, 0, buffer.Length)) != 0))
                {
                    ms.Write(buffer, 0, readbyte);
                }
            }
        }

        ms.Position = 0;
        BinaryReader reader = new BinaryReader(ms); 
        int version = reader.ReadInt32();
        bool encrypt = reader.ReadBoolean();
        //check version
        if (version < (int)LocalDataVersion.ATLEAST)
        {
            reader.Close();
            ms.Close();
            //File.Delete(filepath);
            return default(T);
        }

        if (encrypt)
        {
            if (cryptkey == null || cryptkey.Length != 32)
                throw new ArgumentNullException("cryptkey == null || cryptkey.Length != 32");
            //check secret
            string secret = reader.ReadString();
            if (secret != MD5Helper.GetHash(cryptkey))
            {
                reader.Close();
                ms.Close();
                return default(T);
            }
        }

        byte[] data = reader.ReadBytes((int)(ms.Length - ms.Position));
        ms.Close();
        reader.Close();

        if (encrypt)
        {
            data = AESHelper.Decrypt(data, cryptkey, cryptvector);
        }

        T ret = Deserialize<T>(data, version);
        return ret;
    }

    public static byte[] Serialize<T>(T data, int version) where T : ILocalSerializable, new()
    {
        if (data == null)
            throw new ArgumentException("data");

        byte[] serialized = null;
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    LocalSerializationContext context = new LocalSerializationContext();
                    context.version = version;
                    context.writer = bw;
                    data.Serialize(context);
                    serialized = ms.ToArray();
                }
            }
        }
        catch
        {
            throw new Exception("Local data serialization exception");
        }
        return serialized;
    }

    public static T Deserialize<T>(byte[] data, int version) where T : ILocalSerializable, new()
    {
        T t = new T();
        using (MemoryStream ms = new MemoryStream(data))
        {
            using (BinaryReader br = new BinaryReader(ms))
            {
                LocalSerializationContext context = new LocalSerializationContext();
                context.version = version;
                context.reader = br;
                t.Deserialize(context);
            }
        }
        return t;
    }

    public static void Delete(string filename)
    {
        string filepath = Path.Combine(localDataPath, filename);
        if (File.Exists(filepath))
        {
            File.Delete(filepath);
        }
    }
}


