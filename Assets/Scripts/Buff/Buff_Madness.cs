using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Madness : Buff
{
    public float effectDuration;
    public float timer_EffectDuration;

    public bool isTrigger;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        id = 2;
        name = "Madness";

        permanent = true;
        debuff = true;

        triggerTime = 75.0f;
        triggerTimeRandomRange = 15.0f;
        timer_triggerTime = Random.Range(triggerTime - triggerTimeRandomRange, triggerTime + triggerTimeRandomRange);

        effectDuration = 3.0f;
        timer_EffectDuration = effectDuration;

        isTrigger = false;

        CreateBuffImage(id);
    }

    void Update()
    {
        if (isTrigger) {
            if (timer_EffectDuration <= 0)
            {
                timer_EffectDuration = effectDuration;
                GameObject.Find("Canvas_UI").transform.Find("Joystick").GetComponent<Joystick>().SetControllable(true);
                isTrigger = false;
            }
            else {
                timer_EffectDuration -= Time.deltaTime;
            }
        }

        Timer_TriggerEffect();
    }

    public override void Effect()
    {
        isTrigger = true;
        GameObject.Find("Canvas_UI").transform.Find("Joystick").GetComponent<Joystick>().SetControllable(false);
    }
}
