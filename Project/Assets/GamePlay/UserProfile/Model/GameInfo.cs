using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInfo : ILocalSerializable
{
    public int level { get; set; }
    public int gold { get; set; }
    public int gem { get; set; }
    public int energy { get; set; }
    public long lastEnergyRechargeTime { get; set; }
    public int onlineEnergy { get; set; }
    public int experience { get; set; }
    public Dictionary<string, InventoryItem> inventoryItems { get; set; }
   

    public GameInfo()
    {
        this.level = 0;
        this.gold = 0;
        this.gem = 0;
        this.energy = 0;
        this.lastEnergyRechargeTime = -1;
        this.experience = 0;
        this.inventoryItems = new Dictionary<string, InventoryItem>();
    }

    #region Serialize
    public void Serialize(LocalSerializationContext context)
    {
        context.writer.Write(level);
        context.writer.Write(gold);
        context.writer.Write(gem);
        context.writer.Write(energy);
        context.writer.Write(lastEnergyRechargeTime);
        context.writer.Write(experience);

    }

    public void Deserialize(LocalSerializationContext context)
    {
        level = context.reader.ReadInt32();
        gold = context.reader.ReadInt32();
        gem = context.reader.ReadInt32();
        energy = context.reader.ReadInt32();
        lastEnergyRechargeTime = context.reader.ReadInt64();
        experience = context.reader.ReadInt32();

        
    }
    #endregion

}

