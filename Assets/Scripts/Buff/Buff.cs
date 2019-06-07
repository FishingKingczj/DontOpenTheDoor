using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    [Header("Basic Infomation")]
    public int id;
    public string name;

    public bool permanent;

    public float triggerTime;
    public float triggerTimeRandomRange;
    public float timer_triggerTime;

    public float duration;

    // 效果
    public virtual void Effect() { }
    // 移除效果
    public virtual void RemoveEffect() { }

    // 定时器(移除BUFF 针对非永久性增益BUFF)
    public virtual void Timer_RemoveBuff() {
        if (permanent == false) {
            if (duration <= 0)
            {
                RemoveEffect();
                Destroy(this);
            }
            else {
                duration -= Time.deltaTime;
            }
        }
    }

    // 定时器(触发效果 针对永久性触发BUFF)
    public virtual void Timer_TriggerEffect() {
        if (timer_triggerTime <= 0)
        {
            Effect();
            timer_triggerTime = Random.Range(triggerTime - triggerTimeRandomRange, triggerTime + triggerTimeRandomRange);
        }
        else {
            timer_triggerTime -= Time.deltaTime;
        }
    }

    public virtual void SetDuration(float _value) {
        duration = _value;
    }
}
