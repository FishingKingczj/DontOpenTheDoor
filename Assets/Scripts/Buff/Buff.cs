using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff : MonoBehaviour
{
    [Header("Basic Infomation")]
    public int id;
    public string name;

    public bool permanent;
    public bool debuff;

    public float triggerTime;
    public float triggerTimeRandomRange;
    public float timer_triggerTime;

    public float duration;

    public Sprite buffImage;
    private GameObject buffImage_Object;

    public virtual void Start()
    {
    }

    // 效果
    public virtual void Effect() { }
    // 移除效果
    public virtual void RemoveEffect() { }

    // 定时器(移除BUFF 针对非永久性增益BUFF)
    public virtual void Timer_RemoveBuff() {
        if (permanent == false) {
            if (duration <= 0)
            {
                RemoveEffect();
                Destroy(this);
            }
            else {
                duration -= Time.deltaTime;
            }
        }
    }

    // 定时器(触发效果 针对永久性触发BUFF)
    public virtual void Timer_TriggerEffect() {
        if (timer_triggerTime <= 0)
        {
            Effect();
            timer_triggerTime = Random.Range(triggerTime - triggerTimeRandomRange, triggerTime + triggerTimeRandomRange);
        }
        else {
            timer_triggerTime -= Time.deltaTime;
        }
    }

    public virtual void SetDuration(float _value) {
        duration = _value;
    }

    public void CreateBuffImage(int _id) {
        buffImage_Object = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Image_Buff"));
        buffImage_Object.transform.SetParent(GameObject.Find("Canvas_UI").transform.Find("Panel_Buff").transform);

        switch (_id) 
        {
            case 0:
                {
                    buffImage_Object.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/UI/Buff/幻视");
                    break;
                }
            case 1:
                {
                    buffImage_Object.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/UI/Buff/幻听");
                    break;
                }
            case 2:
                {
                    buffImage_Object.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/UI/Buff/疯狂");
                    break;
                }
            case 3:
                {
                    buffImage_Object.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/UI/Buff/疲惫");
                    break;
                }
            case 4:
                {
                    buffImage_Object.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/UI/Buff/脆弱");
                    break;
                }
            case 5:
                {
                    break;
                }
            default: break;
        }
    }

    public bool GetDebuff() { return debuff; }

    public virtual void OnDestroy()
    {
        Destroy(buffImage_Object);
    }
}
