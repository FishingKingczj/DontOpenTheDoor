using System.Collections;
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
        description = "这是个钥匙";

        pickable = true;

        maxStorageAmount = 1;

        usageTime = 2.0f;
        timer_UsageTime = usageTime;

        progressRing = GameObject.Find("Canvas_UI").transform.Find("ProgressRing").GetComponent<Image>();
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
                        Debug.Log("这钥匙不是开这里的");
                        return;
                    }
                    if (t.GetComponent<Item_Door>().inOpened)
                    {
                        Debug.Log("门开着");
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

                    progressRing.fillAmount = (usageTime - timer_UsageTime) / usageTime;
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
