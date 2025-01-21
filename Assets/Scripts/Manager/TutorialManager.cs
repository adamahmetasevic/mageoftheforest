using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private Dictionary<string, bool> tutorialTriggers = new Dictionary<string, bool>();

    public bool GetTriggerState(string triggerName)
    {
        return tutorialTriggers.ContainsKey(triggerName) && tutorialTriggers[triggerName];
    }

    public void SetTriggerState(string triggerName, bool state)
    {
        if (!tutorialTriggers.ContainsKey(triggerName))
        {
            tutorialTriggers.Add(triggerName, state);
        }
        else
        {
            tutorialTriggers[triggerName] = state;
        }
    }

    public Dictionary<string, bool> GetAllTriggers()
    {
        return new Dictionary<string, bool>(tutorialTriggers);
    }

    public void SetAllTriggers(Dictionary<string, bool> triggers)
    {
        tutorialTriggers = new Dictionary<string, bool>(triggers);
    }
}
