using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Food : Item
{
    private const float DEFAULT_ENERGYRESTORE = 10.0f;

    void Start()
    {
        id = 0;
        name = "Food";
        description = "这是一袋大米,恢复10点体力";

        pickable = true;

        usageTime = 2.0f;
        timer_UsageTime = usageTime;

        maxStorageAmount = 5;

        progressRing = GameObject.Find("ProgressRing").GetComponent<Image>();
    }

    // 使用效果
    public override void Effect(GameObject _user)
    {
        base.Effect(_user);
        _user.GetComponent<Player>().AddEnergy(DEFAULT_ENERGYRESTORE);
    }
}
