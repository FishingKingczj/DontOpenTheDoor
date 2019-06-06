using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff_Vision : Buff
{
    // Start is called before the first frame update
    void Start()
    {
        id = 5;
        name = "Vision";

        permanent = true;

        Effect();
    }

    public override void Effect()
    {
        base.Effect();
        Camera.main.cullingMask |= (1 << 8);

        // 遍历所有不可见层对象
        foreach (GameObject t in FindObjectsOfType(typeof(GameObject)) as GameObject[]) {
            if (t.layer != 8) continue;


        }
    }

    public override void RemoveEffect()
    {
        base.RemoveEffect();
        Camera.main.cullingMask &= ~(1 << 8);
    }

    public void OnDestroy()
    {
        RemoveEffect();
    }
}
