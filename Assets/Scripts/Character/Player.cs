﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MoveObject
{
	
	private const float DEFAULT_MAXENERGY = 100.0f;
	[Header("Energy varible")]
	public float energy_Current = DEFAULT_MAXENERGY;

	public float energyConsumption_Idle =  0.33f;
	public float energyConsumption_Walk =  0.33f;
	public float energyConsumption_Rush =  5.0f;

    //活力条
	public Slider energy_Slider;

    [Header("Interact varible")]
    public float interaction_Range = 1.2f;

    [Header("Movement varible")]
    public bool inMoved = false;

    [Header("EscapeMode varible")]
    public GameObject monster;

    public bool inEscape = false;

    [Range(0,MAXESCAPEPOINT)]
    public float escapePoint = 0.0f;

    private const float STARTESCAPEUSAGETIME = 1.0f;
    private const float ESCAPEPOINTCOST = 3.0f;
    private const float MAXESCAPEPOINT = 50.0f;
    public float timer_StartEscape = STARTESCAPEUSAGETIME;

    public Slider escapePoint_Slider;
    public Joystick joystick;

    private const float DEFAULT_SPEED = 1f;
    private const float RUSH_SPEED = DEFAULT_SPEED * 1.5f;

	void Start(){
        //CircleCollider2D box = gameObject.AddComponent<CircleCollider2D>();
        //box.radius = interaction_Range;
        //box.isTrigger = true;

        energy_Slider = GameObject.Find("Slider_Energy").GetComponent<Slider>();

        escapePoint_Slider = GameObject.Find("Canvas_UI").transform.Find("Slider_EscapePoint").gameObject.GetComponent<Slider>();


        joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
    }

    void FixedUpdate()
    {
        if (inEscape) {
            Escape();
            return;
        }

        CheckSpeed();

#if UNITY_ANDROID
#endif

#if UNITY_IPHONE
#endif

#if UNITY_STANDALONE_WIN
            PlayerRush();
            PlayerMove();
            PlayerInteract();
#endif
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("你碰到了" + collider.name.ToString());
    }

    // 接触结束
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider != null)
        {
            //Debug.Log("你离开了" + collider.name.ToString());
        }
    }

    // 控制人物移动
    private void PlayerMove()
    {
        if (energy_Current <= 0 || this.GetComponent<Player_BackPack>().GetInCompositeMode()) return;

        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        Vector3 vector = new Vector3(dx, dy);

        if (vector != Vector3.zero) SetInMoved(true);
        else SetInMoved(false);

        Move(vector);
    }

    // 控制人物移动(移动端)
    public void PlayerMove(Vector3 dir)
    {
        if (energy_Current <= 0 || this.GetComponent<Player_BackPack>().GetInCompositeMode()) return;

        Vector3 vector = dir;

        Move(vector);
    }

    // 控制人物冲刺
    private void PlayerRush()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            setSpeed(RUSH_SPEED);
        }
        else
        {
            setSpeed(DEFAULT_SPEED);
        }
    }

    // 控制人物冲刺(移动端)
    public void PlayerRush(bool inRushed)
    {
        if (inRushed)
        {
            setSpeed(RUSH_SPEED);
        }
        else
        {
            setSpeed(DEFAULT_SPEED);
        }
    }

    // 控制玩家互动
    private void PlayerInteract() {
        if (inMoved || inEscape) return;

        // 如果玩家正在使用物品，屏蔽地图交互
        if (GetComponent<Player_BackPack>().inSelected)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, interaction_Range);
            if (colliders.Length > 0)
            {
                Collider2D item = null;
                float dis = 0x9f9f9f9f;

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (!colliders[i].tag.Contains("Item"))
                        continue;
                    else
                    {
                        if (Vector2.Distance(this.gameObject.transform.position, colliders[i].transform.position) < dis)
                        {
                            dis = Vector2.Distance(this.gameObject.transform.position, colliders[i].transform.position);
                            item = colliders[i];
                        }
                    }
                }

                if (item == null) { return; }
                else item.SendMessage("Interact", this.gameObject, SendMessageOptions.DontRequireReceiver); 
            }
        }
    }

    // 控制玩家互动(移动端)
    public void PlayerInteract(int _index)
    {
        if (inMoved || inEscape) return;

        // 如果玩家正在使用物品，屏蔽地图交互
        if (GetComponent<Player_BackPack>().inSelected)
        {
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, interaction_Range);
        if (colliders.Length > 0)
        {
            Collider2D item = null;
            float dis = 0x9f9f9f9f;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i].tag.Contains("Item"))
                    continue;
                else
                {
                    if (Vector2.Distance(this.gameObject.transform.position, colliders[i].transform.position) < dis)
                    {
                        dis = Vector2.Distance(this.gameObject.transform.position, colliders[i].transform.position);
                        item = colliders[i];
                    }
                }
            }

            if (item == null) { return; }
            else item.SendMessage("Interact", this.gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }

    // 增加体力
    public void AddEnergy(float _value = 1) {
        energy_Current += _value;
        energy_Slider.value = energy_Current;
    }
    // 减少体力
    public void ReduceEnergy(float _value = 1) {
        energy_Current -= _value;
        energy_Slider.value = energy_Current;
    }
    // 增加逃生点
    public void AddEscapePoint(float _value = 1) {
        if(inEscape && (joystick.GetVector() == Vector2.zero))
        escapePoint += _value;
    }
    // 减少逃生点
    public void ReduceEscapePoint(float _value = 1) {
        escapePoint -= _value;
    }

    // 检测当前速度
    private void CheckSpeed() {
        // 检测是否移动 (玩家体力消耗)
        if (GetInMoved())
        {
            // 检测奔跑速度
            if (GetSpeed() == DEFAULT_SPEED)
                energy_Current -= energyConsumption_Walk * Time.deltaTime;
            else
                energy_Current -= energyConsumption_Rush * Time.deltaTime;
        }
        else
        {
            energy_Current -= energyConsumption_Idle * Time.deltaTime;
        }
        energy_Current = Mathf.Clamp(energy_Current, 0, DEFAULT_MAXENERGY);
        energy_Slider.value = energy_Current;
    }

    // 玩家格挡
    public void PlayerBlock() {
        monster.SendMessage("PlayerBlock", SendMessageOptions.DontRequireReceiver);
    }

    // 进入/退出 逃脱模式
    public void EnterEscapeMode(GameObject _monster) {
        Debug.Log("玩家进入逃生模式");
        escapePoint = 0;
        monster = _monster;
        inEscape = true;

        timer_StartEscape = STARTESCAPEUSAGETIME;

        GameObject.Find("Canvas_UI").transform.Find("Button_Block").gameObject.SetActive(true);
        GameObject.Find("Canvas_UI").transform.Find("Slider_EscapePoint").gameObject.SetActive(true);

        // 清除背包中任意操作
        this.GetComponent<Player_BackPack>().ExitMultipleSelectMode();
    }
    public void ExitEscapeMode() {
        Debug.Log("玩家离开逃生模式");
        monster.SendMessage("EndAttack", SendMessageOptions.DontRequireReceiver);

        monster = null;
        inEscape = false;

        GameObject.Find("Canvas_UI").transform.Find("Button_Block").gameObject.SetActive(false);
        GameObject.Find("Canvas_UI").transform.Find("Slider_EscapePoint").gameObject.SetActive(false);
    }

    // 逃脱模式检测
    public void Escape()
    {
        if (timer_StartEscape <= 0)
        {
            escapePoint -= ESCAPEPOINTCOST * Time.deltaTime;

            if (escapePoint <= 0)
            {
               // Debug.Log("玩家死亡 逃生点 0");
               // ExitEscapeMode();
            }

            else if (escapePoint >= 50) {
                Debug.Log("玩家逃脱 逃生点 50");
                ExitEscapeMode();
            }
        }
        else
        {
            timer_StartEscape -= Time.deltaTime;
        }

        escapePoint_Slider.value = escapePoint;
    }

    public void SetInMoved(bool _moved) { inMoved = _moved; }
    public bool GetInMoved() { return inMoved; }

    public bool GetInEscape() { return inEscape; }
}
