using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_SpecialFood : Item
{
    private const float DEFAULT_ENERGYRESTORE = 50.0f;
    private const float DEFAULT_PRESSUREPOINTINCREMENT = 10.0f;

    void Awake()
    {
        id = 8;
        name = "SpecialFood";
        description = "This food is made by alchemists using a raw material that people don't want to talk about.";

        pickable = true;

        usageTime = 2.0f;
        timer_UsageTime = usageTime;

        maxStorageAmount = 5;
    }

    // 使用效果
    public override void Effect(GameObject _user)
    {
        base.Effect(_user);
        _user.GetComponent<Player>().AddEnergy(DEFAULT_ENERGYRESTORE);
        _user.GetComponent<Player_BuffManager>().AddPressurePoint(DEFAULT_PRESSUREPOINTINCREMENT);
    }
}
