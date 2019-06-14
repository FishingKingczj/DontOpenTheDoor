using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressRing : MonoBehaviour
{
    private static GameObject player;
    private static GameObject progressRing_Object;
    private static Slider progressRing;
    private static bool inTrigger;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        progressRing_Object = GameObject.Find("Canvas_UI_WorldSpace").transform.Find("ProgressRing").gameObject;
        progressRing = progressRing_Object.GetComponent<Slider>();
        Reset();
    }

    private void FixedUpdate()
    {
        progressRing.transform.position = player.transform.position + new Vector3(0, 2.5f, 0);
    }

    public static void Use(float _value) {
        progressRing.value = _value;
        inTrigger = true;
    }
    public static void Reset()
    {
        progressRing.value = 0;
        inTrigger = false;
    }
}
