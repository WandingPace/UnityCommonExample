using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Enum)]
public class PropertiesDesc : System.Attribute
{
    public PropertiesDesc(string desc)
    {
        Desc = desc;
    }
    public string Desc { get; private set; }

}
public class PropertiesUtils
{
    public static string GetDescByProperties(Properties p)
    {
        Type type = p.GetType();
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo field in fields)
        {
            if (field.Name.Equals(p.ToString()))
            {
                object[] objs = field.GetCustomAttributes(typeof(PropertiesDesc), true);
                if (objs != null && objs.Length > 0)
                {
                    return ((PropertiesDesc)objs[0]).Desc;
                }
                else
                {
                    return p.ToString() + "没有附加PropertiesDesc信息";
                }
            }
        }
        return "No Such field : " + p;
    }
}  

public enum Properties
{

    [PropertiesDesc("血量")]
    HP = 1,

    [PropertiesDesc("物理攻击")]
    PhyAtk = 2,

    [PropertiesDesc("物理防御")]
    PhyDef = 3,

    [PropertiesDesc("法术攻击")]
    MagAtk = 4,

    [PropertiesDesc("法术防御")]
    MagDef = 5
}