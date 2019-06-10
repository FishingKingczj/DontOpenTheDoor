using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Fatigue : Buff
{
    public float extraEnerguConsumption = 2.0f;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        id = 3;
        name = "Fatigue";

        permanent = true;
        debuff = true;

        Effect();

        CreateBuffImage(id);
    }

    public override void Effect()
    {
        base.Effect();

        this.GetComponent<Player>().AddEnergyExtraConsumption(extraEnerguConsumption);
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();

        this.GetComponent<Player>().ReduceEnergyExtraConsumption(extraEnerguConsumption);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        RemoveEffect();
    }
}
