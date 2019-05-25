﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [Header("Basic Infomation")]
    public int id;
    public string name;
    public string description;

    public bool pickable;

    public int maxStorageAmount;
    public float usageTime;
    public float timer_UsageTime;

    // 使用进度环(UI)
    public Image progressRing;

    // 互动效果
    public virtual void Interact(GameObject _user){
        //拾取类物品
        if (pickable == true)
        {
            if (_user.GetComponent<Player_BackPack>().AddItem(this.gameObject, maxStorageAmount, id ,name, description))
                Destroy(this.gameObject);
            else {
                Debug.Log("背包已满");
            }
        }
        //非拾取类互动物品
        else {
           
        }
    }

    // 使用效果
    public virtual void Effect(GameObject _user) { }

    // 使用和重设使用时间
    public virtual void Use(GameObject _user) {
        if (timer_UsageTime <= 0)
        {
            Effect(_user);
            _user.GetComponent<Player_BackPack>().UseSucceed();
            Use_Reset();
        }
        else {
            timer_UsageTime -= Time.deltaTime;
        }

        progressRing.fillAmount = (usageTime - timer_UsageTime) / usageTime;
    }
    public virtual void Use_Reset() {
        timer_UsageTime = usageTime;

        progressRing.fillAmount = 0;
    }
}