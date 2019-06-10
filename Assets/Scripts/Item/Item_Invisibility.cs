using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Invisibility : Item
{
    [Header("Additional Variable")]
    public float duration;

    void Awake()
    {
        id = 7;
        name = "Invisibility";
        description = "这是一瓶隐形药水";

        pickable = true;

        maxStorageAmount = 5;

        usageTime = 2.0f;
        timer_UsageTime = usageTime;

        duration = 60.0f;

        progressRing = GameObject.Find("Canvas_UI").transform.Find("ProgressRing").GetComponent<Image>();
    }

    public override void Effect(GameObject _user)
    {
        _user.gameObject.AddComponent<Buff_Invisibility>();
        _user.gameObject.GetComponent<Buff_Invisibility>().SetDuration(duration);
    }
}
