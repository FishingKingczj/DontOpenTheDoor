using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Fatigue : Buff
{
    public float extraEnerguConsumption = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        id = 3;
        name = "Fatigue";

        permanent = true;

        Effect();
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

    public void OnDestroy()
    {
        RemoveEffect();
    }
}
