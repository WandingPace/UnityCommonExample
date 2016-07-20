using UnityEngine;
using System.Collections;

public class UserInfo : ILocalSerializable
{
    public string nickname = "";
    public string iconId = "";
    public string iconUrl = "";
    
    public UserInfo()
    {
        this.nickname = "";
        this.iconId = "";
        this.iconUrl = "";
    }

    public void Serialize(LocalSerializationContext context)
    {
        context.writer.Write(nickname);
        context.writer.Write(iconId);
        context.writer.Write(iconUrl);
    }

    public void Deserialize(LocalSerializationContext context)
    {
        nickname = context.reader.ReadString();
        iconId = context.reader.ReadString();
        iconUrl = context.reader.ReadString();
    }
}
