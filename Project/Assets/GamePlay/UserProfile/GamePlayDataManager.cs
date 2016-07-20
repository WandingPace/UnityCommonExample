using UnityEngine;
using System.Collections;

public class GamePlayDataManager
{
    #region Singleton
    private static readonly GamePlayDataManager _instance = new GamePlayDataManager();
    public static GamePlayDataManager instance { get { return _instance; } }
    private GamePlayDataManager() { }
    #endregion

    #region Constant Value
    private const string BOX_TEMPLATEINFO_XML_FILENAME = "BoxReward";
    private const string POWER_CARD_TEMPLATEINFO_XML_FILENAME = "PowerCard";
    private const string DOBBLE_CARD_TEMPLATEINFO_XML_FILENAME = "DobbleCard";
    private const string PLAYER_LEVEL_EXP_FILENAME = "PlayerLevel";
    private const string SYMBOLE_TEMPLATEINFO_XML_FILENAME = "Symbols";
    #endregion

    #region Data
    //public Dictionary<string, RewardBoxTemplateInfo> rewardBoxInfos = new Dictionary<string, RewardBoxTemplateInfo>();
    //public Dictionary<string, PowerCardTemplateInfo> powerCardInfos = new Dictionary<string, PowerCardTemplateInfo>();
    //public Dictionary<string, DobbleCardTemplateInfo> dobbleCardInfos = new Dictionary<string, DobbleCardTemplateInfo>();
    //public Dictionary<string, SymbolTemplateInfo> symbolInfos = new Dictionary<string, SymbolTemplateInfo>();
    //public Dictionary<int, int> levelExpInfos = new Dictionary<int, int>();
    #endregion

    #region Interface Functions.
    public void LoadData()
    {

    }

    public int GetLevelByExp(int exp)
    {
        return 0;
    }
    #endregion

    #region Private Functions



    private int[] ToIntArray(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length < 3)
            return new int[0];

        string num = value.Substring(1, value.Length - 2);
        string[] nums = num.Split(new char[] { ',' });
        int[] values = new int[nums.Length];
        for (int i = 0; i < values.Length; ++i)
        {
            int.TryParse(nums[i], out values[i]);
        }
        return values;
    }
    #endregion
}
