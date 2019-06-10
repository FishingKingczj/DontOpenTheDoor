using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Tranquilizer : Item
{
    // Start is called before the first frame update
    void Awake()
    {
        id = 4;
        name = "Tranquilizer";
        description = "这是一瓶镇定剂,消除一个Debuff";

        pickable = true;

        usageTime = 2.0f;
        timer_UsageTime = usageTime;

        maxStorageAmount = 5;
    }

    public override void Effect(GameObject _user)
    {
        base.Effect(_user);
        _user.GetComponent<Player_BuffManager>().DeleteBuff();
    }
}
