using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_BuffManager : MonoBehaviour
{
    [Header("Basic variable")]
    public float pressurePoint = 0;

    public List<PressureThreshold> pressureThreshold;
    public int pressureLevel = 0;

    [System.Serializable]
    public struct PressureThreshold {
        public float threshold;
        public List<int> buffID;
    }

    public List<int> currentBuffId;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 增加/减少 压力值(同时检测压力等级)
    public void AddPressurePoint(float _value) {
        pressurePoint += _value;

        // 临时使用
        GameObject.Find("Text_PressurePoint").GetComponent<Text>().text = "PreesurePoint : " + pressurePoint + "\nPressureLevel : " + pressureLevel;

        if (pressureLevel != pressureThreshold.Count && pressurePoint >= pressureThreshold[pressureLevel].threshold)
        {
            pressureLevel++;
            AddBuff();
        }
    }
    public void ReducePressPoint(float _value) {
        pressurePoint -= _value;

        // 临时使用
        GameObject.Find("Text_PressurePoint").GetComponent<Text>().text = "PreesurePoint : " + pressurePoint + "\nPressureLevel : " + pressureLevel;

        if (pressureLevel > 0 && pressurePoint < pressureThreshold[pressureLevel - 1].threshold)
        {
            pressureLevel--;
        }
    }

    // 添加Buff
    public void AddBuff() {
        if (pressureLevel < 1) return;

        int size = pressureThreshold[pressureLevel - 1].buffID.Count;

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
                    case 5:
                        {
                            break;
                        }
                    default:break;
                }
                currentBuffId.Add(newBuffId);

                return;
            }
        }

        Debug.Log("效果重叠");
    }

    // 删除Buff
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
            case 5:
                {
                    break;
                }
            default: break;

        }

        currentBuffId.Remove(deleteIndex);
    }
}
