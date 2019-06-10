using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_BuffManager : MonoBehaviour
{
    [Header("Basic variable")]
    public float pressurePoint = 0;
    public float pressurePointLine = 0;
    private const float CHANGESPEED = 40.0f;
    private const float MAXPRESSUREPOINT = 200.0f;

    public List<PressureThreshold> pressureThreshold;
    public int pressureLevel = 0;

    [System.Serializable]
    public struct PressureThreshold {
        public float threshold;
        public List<int> buffID;
    }

    public List<int> currentBuffId;

    public Slider slider;

    private void Start()
    {
        slider = GameObject.Find("Canvas_UI").transform.Find("Slider_PressurePoint").GetComponent<Slider>();
    }

    void Update()
    {
        CheckPressPoint();
    }

    // 增加/减少 压力值(同时检测压力等级)
    public void AddPressurePoint(float _value) {
        pressurePointLine += _value;
    }
    public void ReducePressPoint(float _value) {
        pressurePointLine -= _value;
    }

    // 添加Buff(随机池)
    public void AddBuff() {
        if (pressureLevel < 1) return;

        int size = pressureThreshold[pressureLevel - 1].buffID.Count;

        Debug.Log("test");
        for (int i = 0; i < 20; i++) {
            int newBuffId = pressureThreshold[pressureLevel - 1].buffID[Random.Range(0, size)];

            if (!currentBuffId.Contains(newBuffId)) {
                switch (newBuffId) {
                    case 0:
                        {
                            this.gameObject.AddComponent<Buff_Phantom>();
                            break;
                        }
                    case 1:
                        {
                            this.gameObject.AddComponent<Buff_AuditoryHallucination>();
                            break;
                        }
                    case 2:
                        {
                            this.gameObject.AddComponent<Buff_Madness>();
                            break;
                        }
                    case 3: {
                            this.gameObject.AddComponent<Buff_Fatigue>();
                            break;
                        }
                    case 4:
                        {
                            this.gameObject.AddComponent<Buff_Frangibility>();
                            break;
                        }
                    default: break;
                }
                currentBuffId.Add(newBuffId);
                return;
            }
        }

        Debug.Log("效果重叠");
    }

    // 删除Buff(随机池)
    public void DeleteBuff() {
        if (currentBuffId.Count == 0) return;

        int deleteIndex = Random.Range(0, currentBuffId.Count);

        int deleteId = currentBuffId[deleteIndex];
        switch (deleteId)
        {
            case 0:
                {
                    Destroy(this.gameObject.GetComponent<Buff_Phantom>());
                    break;
                }
            case 1:
                {
                    Destroy(this.gameObject.GetComponent<Buff_AuditoryHallucination>());
                    break;
                }
            case 2:
                {
                    Destroy(this.gameObject.GetComponent<Buff_Madness>());
                    break;
                }
            case 3:
                {
                    Destroy(this.gameObject.GetComponent<Buff_Fatigue>());
                    break;
                }
            case 4:
                {
                    Destroy(this.gameObject.GetComponent<Buff_Frangibility>());
                    break;
                }
            default: break;

        }
        currentBuffId.Remove(deleteIndex);
    }

    private void CheckPressPoint() {
        pressurePointLine = Mathf.Clamp(pressurePointLine, 0, 200.0f);

        if (pressurePointLine > pressurePoint)
        {
            pressurePoint += CHANGESPEED * Time.deltaTime;
            pressurePoint = Mathf.Clamp(pressurePoint, pressurePoint, pressurePointLine);
        }
        else if (pressurePointLine < pressurePoint) {
            pressurePoint -= CHANGESPEED * Time.deltaTime;
            pressurePoint = Mathf.Clamp(pressurePoint, pressurePointLine, pressurePoint);
        }

        slider.value = pressurePoint;

        // 检测压力等级
        if (pressureLevel != pressureThreshold.Count && pressurePoint >= pressureThreshold[pressureLevel].threshold)
        {
            pressureLevel++;
            AddBuff();
        }
        if (pressureLevel > 0 && pressurePoint < pressureThreshold[pressureLevel - 1].threshold)
        {
            pressureLevel--;
        }
    }
}
