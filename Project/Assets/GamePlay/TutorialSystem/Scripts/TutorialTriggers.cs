using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Base class of all tutorial triggers.
/// Tutorial triggers are what trigers a tutorial to start.
/// </summary>
public abstract class TutorialTrigger
{
    public abstract void Initialize(string[] parameters);
    public abstract bool IsTriggered(object data);
}

public class GameStartTutorialTrigger : TutorialTrigger
{
    public override void Initialize(string[] parameters)
    {
        // No parameters for this trigger.
    }

    public override bool IsTriggered(object data)
    {
        return true;
    }
}

public class PVELevelTutorialTrigger : TutorialTrigger
{
    int pveLevelId;
    int minDobbleCount;
    int minPowerCount;

    public override void Initialize(string[] parameters)
    {
        if (parameters.Length < 1 || parameters.Length > 3) throw new InvalidOperationException();

        if (parameters.Length == 1)
        {
            this.pveLevelId = Convert.ToInt32(parameters[0]);
            this.minDobbleCount = 0;
            this.minPowerCount = 0;
        }
        else if(parameters.Length == 2)
        {
            this.pveLevelId = -1;
            this.minDobbleCount = Convert.ToInt32(parameters[0]);
            this.minPowerCount = Convert.ToInt32(parameters[1]);
        }
        if(parameters.Length == 3)
        {
            this.pveLevelId = Convert.ToInt32(parameters[0]);
            this.minDobbleCount = Convert.ToInt32(parameters[1]);
            this.minPowerCount = Convert.ToInt32(parameters[2]);
        }
    }

    public override bool IsTriggered(object data)
    {

    //    int dobbleCount = UserProfile.current.gameInfo.dobbleInfos.Count;
    //    int powerCount = UserProfile.current.gameInfo.powerInfos.Count;

    //    bool isLevelMatched = (pveLevelId == -1 || (Convert.ToInt32(data) == pveLevelId));
    //    bool hasSufficientDobblePower = (dobbleCount >= minDobbleCount) && (powerCount >= minPowerCount);

    //    return isLevelMatched && hasSufficientDobblePower;
        return false;
    }
}

public class EnterPVELevelTutorialTrigger : PVELevelTutorialTrigger
{
    // Logic is same as PVELevelTutorialTrigger.
    // Only difference is that it's triggered at a different time.
}