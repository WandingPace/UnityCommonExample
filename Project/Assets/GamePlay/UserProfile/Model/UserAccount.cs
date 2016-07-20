using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserAccount : ILocalSerializable
{
    public string id = "";
    public string username = "";
    public string password = "";//we need encrypted password here.
    public bool authenticated = false;

    public UserAccount()
    {
        this.id = "";
        this.username = "";
        this.password = "";
        this.authenticated = true;
    }

    public void Serialize(LocalSerializationContext context)
    {
        context.writer.Write(id);
        context.writer.Write(username);
        context.writer.Write(password);
        context.writer.Write(authenticated);
    }
    public void Deserialize(LocalSerializationContext context)
    {
        id = context.reader.ReadString();
        username = context.reader.ReadString();
        password = context.reader.ReadString();
        authenticated = context.reader.ReadBoolean();
    }
}
