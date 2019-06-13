using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FadeIn : MonoBehaviour
{
    public float destoryTime= 1.0f;
    public float timer_destoryTime;

    private void Start()
    {
        timer_destoryTime = destoryTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer_destoryTime <= 0)
        {
            Destroy(this);
        }
        else {
            Color c = this.GetComponent<Image>().color;
            c.a = (destoryTime - timer_destoryTime) / destoryTime;
            this.GetComponent<Image>().color = c;

            timer_destoryTime -= Time.deltaTime;
        }
    }
}
