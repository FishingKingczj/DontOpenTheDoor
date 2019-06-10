using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff_Invisibility : Buff
{
    // Start is called before the first frame update
    void Start()
    {
        id = 6;
        name = "Invisibility";

        permanent = false;

        duration = 60.0f;

        Effect();
    }

    // Update is called once per frame
    void Update()
    {
        Timer_RemoveBuff();
    }

    public override void Effect()
    {
        this.gameObject.tag = "Player_Inivisble";
        Color c = this.gameObject.GetComponent<SpriteRenderer>().color;
        c.a = 0.3f;
        this.gameObject.GetComponent<SpriteRenderer>().color = c;
    }

    private void OnDestroy()
    {
        this.gameObject.tag = "Player";
        Color c = this.gameObject.GetComponent<SpriteRenderer>().color;
        c.a = 1.0f;
        this.gameObject.GetComponent<SpriteRenderer>().color = c;
    }
}
