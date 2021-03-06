﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Food : Item
{
    private const float DEFAULT_ENERGYRESTORE = 10.0f;

    void Awake()
    {
        id = 0;
        name = "Food";
        description = "A normal food made by humans hiding in the dungeon relying on only the light source.";

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
    }
}
