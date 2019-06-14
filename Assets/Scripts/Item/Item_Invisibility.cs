using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Invisibility : Item
{
    [Header("Additional Variable")]
    public float duration;

    void Awake()
    {
        id = 7;
        name = "Invisibility";
        description = "Escape from the ancient god’s gaze is to escape death";

        pickable = true;

        maxStorageAmount = 5;

        usageTime = 2.0f;
        timer_UsageTime = usageTime;

        duration = 60.0f;
    }

    public override void Effect(GameObject _user)
    {
        _user.gameObject.AddComponent<Buff_Invisibility>();
        _user.gameObject.GetComponent<Buff_Invisibility>().SetDuration(duration);
    }
}
