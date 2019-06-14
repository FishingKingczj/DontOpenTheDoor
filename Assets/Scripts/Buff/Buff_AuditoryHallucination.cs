using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff_AuditoryHallucination : Buff
{
    [Header("Additional Varible")]
    public AudioSource audio;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        id = 1;
        name = "AuditoryHallucination";

        permanent = true;
        debuff = true;

        triggerTime = 45.0f;
        triggerTimeRandomRange = 15.0f;
        timer_triggerTime = Random.Range(triggerTime - triggerTimeRandomRange, triggerTime + triggerTimeRandomRange);

        audio = this.gameObject.AddComponent<AudioSource>();
        audio.playOnAwake = false;

        CreateBuffImage(id);
    }

    void Update()
    {
        Timer_TriggerEffect();
    }

    public override void Effect()
    { 
        string path = "Audio/Buff/AuditoryHallucination/Audio_" + Random.Range(1, 5).ToString(); // 此处Range最大值根据实际情况修改
        audio.clip = Resources.Load<AudioClip>(path);
        audio.Play();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(audio);
    }
}
