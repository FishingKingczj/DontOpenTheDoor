using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Phantom : Buff
{
    public override void Start()
    {
        base.Start();

        id = 0;
        name = "Phantom";

        permanent = true;
        debuff = true;

        triggerTime = 45.0f;
        triggerTimeRandomRange = 15.0f;
        timer_triggerTime = Random.Range(triggerTime - triggerTimeRandomRange, triggerTime + triggerTimeRandomRange);

        CreateBuffImage(id);
    }

    void Update()
    {
        Timer_TriggerEffect();
    }

    public override void Effect()
    {
        GameObject image = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Buff/Image_Phantom"));
    }
}
