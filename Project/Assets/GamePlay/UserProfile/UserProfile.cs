using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UserProfile : ILocalSerializable
{
    public static UserProfile current = new UserProfile();

    public UserAccount account = new UserAccount();
    public UserInfo userInfo = new UserInfo();
    public GameInfo gameInfo = new GameInfo();

    public void Serialize(LocalSerializationContext context)
    {
        account.Serialize(context);
        userInfo.Serialize(context);
        gameInfo.Serialize(context);
    }

    public void Deserialize(LocalSerializationContext context)
    {
        account.Deserialize(context);
        userInfo.Deserialize(context);
        gameInfo.Deserialize(context);
    }
}

