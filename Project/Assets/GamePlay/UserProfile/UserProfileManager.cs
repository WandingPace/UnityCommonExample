using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UserProfileManager
/// </summary>
public class UserProfileManager
{
    #region Static
    private static readonly UserProfileManager _instance = new UserProfileManager();
    public static UserProfileManager instance
    {
        get
        {
            return _instance;
        }
    }
    private UserProfileManager()
    {
        _secretKey = AESHelper.CreateKey(CRYPTO_KEY);
    }
    #endregion

    #region Private
    private const string CRYPTO_KEY = "www.virtuos.com.key";

    private byte[] _secretKey = null;
    private byte[] secretKey
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

    private string GetFileName(string username)
    {
        return string.Format("{0}.dat", MD5Helper.GetHash(username));
    }
    #endregion

    #region Properties
    public UserProfile profile
    {
        get
        {
            return UserProfile.current;
        }
    }

    public int gold
    {
        get
        {
            return this.profile.gameInfo.gold;
        }
        set
        {
            this.UpdateGold(value);
        }
    }
    public int gem
    {
        get
        {
            return this.profile.gameInfo.gem;
        }
        set
        {
            this.UpdateGem(value);
        }
    }
    public int energy
    {
        get
        {
            return this.profile.gameInfo.energy;
        }
        set
        {
            this.UpdateEnergy(value);
        }
    }
    public int onlineEnergy
    {
        get
        {
            return this.profile.gameInfo.onlineEnergy;
        }
        set
        {
            this.UpdateOnlineEnergy(value);
        }
    }
    public int experience 
    {
        get
        {
            return this.profile.gameInfo.experience;
        }
        set
        {
            this.UpdateExperience(value);
        }
    }
    public int level 
    {
        get 
        {
            return this.profile.gameInfo.level;
        }
        private set 
        {
            this.UpdateLevel(value);
        }
    }
    #endregion

    #region Interfaces.
    public UserProfile Load(string id)
    {
        try
        {
            string filename = GetFileName(id);
            UserProfile profile = LocalDataManager.Load<UserProfile>(filename, this.secretKey);
            UserProfile.current = profile;
            return profile;
        }
        catch 
        {
            return null;
        }
    }
    public void Save(string id, UserProfile profile)
    {
        if (profile == null)
            return;

        string filename = GetFileName(id);
        LocalDataManager.Save<UserProfile>(profile, filename, true, this.secretKey);
    }
    public void Delete(string id)
    {
        string filename = GetFileName(id);
        LocalDataManager.Delete(id);
    }
    public void Save()
    {
        if (UserProfile.current != null && !string.IsNullOrEmpty(UserProfile.current.account.id))
            this.Save(UserProfile.current.account.id, UserProfile.current);
    }
    public void Delete() 
    {
        if (UserProfile.current != null && !string.IsNullOrEmpty(UserProfile.current.account.id))
            this.Delete(UserProfile.current.account.id);
    }

    public void UpdateItems(ICollection<ItemInfo> items) 
    {
        if (items == null || items.Count == 0)
            return;
        foreach (ItemInfo item in items)
        {
            this.UpdateItem(item);
        }
    }
    public void UpdateInventory(ICollection<InventoryItem> inventoryItems)
    {
        if (inventoryItems == null || inventoryItems.Count == 0)
            return;

        bool changed = false;
        foreach (InventoryItem it in inventoryItems) 
        {
            if (it == null)
                continue;

            InventoryItem item = null;
            if (profile.gameInfo.inventoryItems.TryGetValue(it.uid, out item))
            {
                if (item.count != it.count) 
                {
                    item.count = it.count;
                    changed = true;
                    ItemType type = ItemTypeUtilty.GetType(item.id);
                    this.UnlockItems(item.id);
                }
                if (it.count == 0)
                {
                    profile.gameInfo.inventoryItems.Remove(item.uid);
                    changed = true;
                }
            }
            else if(it.count != 0)
            {
                changed = true;
                item = new InventoryItem();
                item.uid = it.uid;
                item.id = it.id;
                item.count = it.count;
                profile.gameInfo.inventoryItems[item.uid] = item;
                this.UnlockItems(item.id);
            }
        }

        if (changed)
            EventDispatcher.Instance.DispatchEvent(EventNames.InventoryChanged, 0);
    }
    public void ModifyItems(ICollection<ItemInfo> items) 
    {
        if (items == null || items.Count == 0)
            return;
        foreach (ItemInfo item in items) 
        {
            this.ModifyItem(item);
        }
    }
    public void ModifyItem(ItemInfo item)
    {
        if (item == null)
            return;

        ItemType type = ItemTypeUtilty.GetType(item.id);
        switch (type)
        {
            case ItemType.Unknown:
                break;
            case ItemType.Gold:
                this.ModifyGold(item.count);
                break;
            case ItemType.Energy:
                this.ModifyEnergy(item.count);
                break;
            case ItemType.Gem:
                this.ModifyGem(item.count);
                break;
            case ItemType.OnlineEnergy:
                this.ModifyOnlineEnergy(item.count);
                break;
            case ItemType.Experience:
                this.ModifyExperience(item.count);
                this.UpdateLevel(GamePlayDataManager.instance.GetLevelByExp(profile.gameInfo.experience));
                break;          
            case ItemType.Box:
                break;

            default:
                break;
        }
    }

    private void UnlockItems(string id) 
    {
        ItemType type = ItemTypeUtilty.GetType(id);
        switch (type)
        {
            
            default:
                break;
        }
    }

    public void UpdateItem(ItemInfo item) 
    {
        if (item == null)
            return;

        ItemType type = ItemTypeUtilty.GetType(item.id);
        switch (type)
        {
            case ItemType.Unknown:
                break;
            case ItemType.Gold:
                this.UpdateGold(item.count);
                break;
            case ItemType.Energy:
                this.UpdateEnergy(item.count);
                break;
            case ItemType.Gem:
                this.UpdateGem(item.count);
                break;
            case ItemType.OnlineEnergy:
                this.UpdateOnlineEnergy(item.count);
                break;
            case ItemType.Experience:
                this.UpdateExperience(item.count);
                this.UpdateLevel(GamePlayDataManager.instance.GetLevelByExp(profile.gameInfo.experience));
                break;
            case ItemType.Box:
                break;
            default:
                break;
        }
    }
    public void UpdateGold(int value)
    {
        if (this.profile.gameInfo.gold != value)
        {
            this.profile.gameInfo.gold = value;
            EventDispatcher.Instance.DispatchEvent(EventNames.GoldChanged, this.profile.gameInfo.gold);
        }
    }
    public void ModifyGold(int change)
    {
        this.UpdateGold(this.profile.gameInfo.gold + change);
    }
    public void UpdateGem(int value) 
    {
        if (this.profile.gameInfo.gem != value)
        {
            this.profile.gameInfo.gem = value;
            EventDispatcher.Instance.DispatchEvent(EventNames.GemChanged, this.profile.gameInfo.gem);
        }
    }
    public void ModifyGem(int change) 
    {
        this.UpdateGem(this.profile.gameInfo.gem + change);
    }
    public void UpdateEnergy(int value) 
    {
        if (this.profile.gameInfo.energy != value)
        {
            this.profile.gameInfo.energy = value;
            EventDispatcher.Instance.DispatchEvent(EventNames.EnergyChanged, this.profile.gameInfo.energy);
        }
    }
    public void ModifyEnergy(int change) 
    {
        this.UpdateEnergy(this.profile.gameInfo.energy + change);
    }
    public void UpdateOnlineEnergy(int value) 
    {
        if (this.profile.gameInfo.onlineEnergy != value)
        {
            this.profile.gameInfo.onlineEnergy = value;
            EventDispatcher.Instance.DispatchEvent(EventNames.OnlineEnergyChanged, this.profile.gameInfo.onlineEnergy);
        }
    }
    public void ModifyOnlineEnergy(int change) 
    {
        this.UpdateOnlineEnergy(this.profile.gameInfo.onlineEnergy + change);
    }
    public void UpdateExperience(int value) 
    {
        if (this.profile.gameInfo.experience != value)
        {
            this.profile.gameInfo.experience = value;
            EventDispatcher.Instance.DispatchEvent(EventNames.ExperienceChanged, this.profile.gameInfo.experience);
        }
    }
    public void ModifyExperience(int change) 
    {
        this.UpdateExperience(this.profile.gameInfo.experience + change);
    }
    private void UpdateLevel(int value) 
    {
        if (this.profile.gameInfo.level != value)
        {
            this.profile.gameInfo.level = value;
            EventDispatcher.Instance.DispatchEvent(EventNames.LevelChanged, this.profile.gameInfo.level);
        }
    }
    #endregion
}
