using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Frangibility : Buff
{
    public float extraEscapePointCost = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        id = 4;
        name = "Frangibility";

        Effect();
    }

    public override void Effect()
    {
        base.Effect();
        this.GetComponent<Player>().AddExtraEscapePointCost(extraEscapePointCost);
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        this.GetComponent<Player>().ReduceExtraEscapePointCost(extraEscapePointCost);
    }

    public void OnDestroy()
    {
        RemoveEffect();
    }
}
