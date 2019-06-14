using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff_Invisibility : Buff
{
    // Start is called before the first frame update
    public override void Start()
    {
        id = 5;
        name = "Invisibility";

        permanent = false;
        debuff = false;

        duration = 60.0f;

        Effect();

        //CreateBuffImage(id);
    }

    // Update is called once per frame
    void Update()
    {
        Timer_RemoveBuff();
    }

    public override void Effect()
    {
        this.gameObject.tag = "Player_Inivisble";
        Color c = this.gameObject.transform.Find("anim").GetComponent<SpriteRenderer>().color;
        c.a = 0.3f;
        this.gameObject.transform.Find("anim").GetComponent<SpriteRenderer>().color = c;
    }

    public override void OnDestroy()
    {
        this.gameObject.tag = "Player";
        Color c = this.gameObject.transform.Find("anim").GetComponent<SpriteRenderer>().color;
        c.a = 1.0f;
        this.gameObject.transform.Find("anim").GetComponent<SpriteRenderer>().color = c;
    }
}
