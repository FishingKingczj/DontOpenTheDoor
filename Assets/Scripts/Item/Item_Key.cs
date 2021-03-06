﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Key : Item
{
    private const float DEFAULT_ENERGYRESTORE = 10.0f;

    [Header("Additional Varible")]
    public int pairingValue;

    void Awake()
    {
        id = 1;
        name = "Key";
        description = "Can lift the magic of blood and meat on specific gates";

        pickable = true;

        maxStorageAmount = 1;

        usageTime = 2.0f;
        timer_UsageTime = usageTime;
    }

    public override void Effect(GameObject _user)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_user.transform.position,Player.interaction_Range);
        if (colliders.Length > 0)
        {
            foreach (Collider2D t in colliders)
            {
                if (t.tag == "Item_Door")
                {
                    if (t.GetComponent<Item_Door>().inLocked)
                    {
                        Debug.Log("开锁成功");

                        _user.GetComponent<Player_BackPack>().UseSucceed(false);
                        t.GetComponent<Item_Door>().Unlock();
                        t.GetComponent<Item_Door>().Open();
                    }
                    else
                    {
                        Debug.Log("上锁成功");

                        _user.GetComponent<Player_BackPack>().UseSucceed(false);
                        t.GetComponent<Item_Door>().Lock();
                    }
                    return;
                }
            }
        }
    }

    public override void Use(GameObject _user)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_user.transform.position, Player.interaction_Range);
        if (colliders.Length > 0)
        {
            foreach (Collider2D t in colliders)
            {
                if (t.tag == "Item_Door")
                {
                    if (!t.GetComponent<Item_Door>().Pair(pairingValue))
                    {
                        Dialog.ShowDialog("This key can't be used here");
                        return;
                    }
                    if (t.GetComponent<Item_Door>().inOpened)
                    {
                        Dialog.ShowDialog("This door is already open");
                        return;
                    }

                    if (timer_UsageTime <= 0)
                    {
                        Effect(_user);
                        Use_Reset();
                    }
                    else
                    {
                        timer_UsageTime -= Time.deltaTime;
                    }

                    ProgressRing.Use((usageTime - timer_UsageTime) / usageTime);
                    return;
                }
            }
        }

        Use_Reset();
    }

    // 设置钥匙配对值
    public void SetPairingValue(int _value) { pairingValue = _value; }
    public int GetPairingValue() { return pairingValue; }
}
